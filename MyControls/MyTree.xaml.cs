using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
    /// MyTree.xaml 的交互逻辑
    /// </summary>
    public partial class MyTree : UserControl
    {
        public MyTree()
        {
            InitializeComponent();
        }


        private void TreeViewMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (TreeNodes.SelectedItem == null)
            {
                return;
            }
            ((sender as ContextMenu).Items[1] as MenuItem).IsEnabled = true;
            ((sender as ContextMenu).Items[2] as MenuItem).IsEnabled = true;
        }


        #region 按钮点击事件

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            Command_AddFolder_Executed(null, null);
        }

        private void BtnAddDoc_Click(object sender, RoutedEventArgs e)
        {
            Command_AddDoc_Executed(null, null);
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            Command_Delete_Executed(null, null);
        }

        private void BtnUnDel_Click(object sender, RoutedEventArgs e)
        {
            Command_UnDel_Executed(null, null);
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            Command_MoveUp_Executed(null, null);
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            Command_MoveDown_Executed(null, null);
        }

        private void BtnKeep_Click(object sender, RoutedEventArgs e)
        {
            Command_Keep_Executed(null, null);
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            Command_Send_Executed(null, null);
        }
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            Command_Import_Executed(sender, null);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Command_Export_Executed(sender, null);
        }
        #endregion

        #region 拖曳移动节点
        /// <summary>
        /// 方法：drop之后向上获取容器
        /// </summary>
        private TreeViewItem GetNearestContainer(UIElement element)
        {
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        /// <summary>
        /// 按下：准备拖曳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodes_DragEnter(object sender, DragEventArgs e)
        {
            TreeViewItem container = GetNearestContainer(e.OriginalSource as UIElement);
            Node dragNode = container.DataContext as Node;
            if (container != null)
            {
                container.Foreground = new SolidColorBrush(Colors.Orange);
            }
        }

        private void TreeNodes_DragLeave(object sender, DragEventArgs e)
        {
            TreeViewItem container = GetNearestContainer(e.OriginalSource as UIElement);
            Node dragNode = container.DataContext as Node;
            if (container != null)
            {
                container.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// 松开：完成拖曳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodes_Drop(object sender, DragEventArgs e)
        {
            TreeViewItem container = GetNearestContainer(e.OriginalSource as UIElement);
            Node dragNode = (sender as TreeView).SelectedValue as Node;
            if (dragNode == null)
            {
                //禁止跨控件拖动
                return;
            }

            Node dropNode = container.DataContext as Node;

            dragNode.MoveTo(dropNode);

            _lastMouseLeftDown = new Point();
            TreeNodes_DragLeave(sender, e);
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodes_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(TextBox) ||
                e.OriginalSource.GetType() == typeof(ScrollViewer) ||
                e.OriginalSource.GetType() == typeof(HandyControl.Controls.ScrollViewer))
            {
                return;
            }
            try
            {
                //获取鼠标选中的节点数据
                Node dragNode = TreeNodes.SelectedItem as Node;
                if (e.LeftButton == MouseButtonState.Pressed && dragNode != null && dragNode.IsDir == false)
                {
                    //获取鼠标移动的距离
                    Point currentPosition = e.GetPosition(TreeNodes);
                    //判断鼠标是否移动
                    if ((Math.Abs(currentPosition.X - _lastMouseLeftDown.X) > 30.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseLeftDown.Y) > 30.0))
                    {
                        //启动拖放操作
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(TreeNodes, TreeNodes.SelectedValue, System.Windows.DragDropEffects.Move);
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("节点移动失败！\n{0}", ex));
            }
        }

        Point _lastMouseLeftDown;

        /// <summary>
        /// 拖动之前的点位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMouseLeftDown = e.GetPosition(this);
        }
        private void TreeNodes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TreeViewItem) == null && _lastReNameNode != null && _lastReNameNode.ReNameing == true)
            {
                _lastReNameNode.FinishRename();
            }
        }
        #endregion


        /// <summary>
        /// 方便在这个控件当中调用的临时变量
        /// </summary>
        TreeViewItem selectedItem = new TreeViewItem();
        private void TreeNodes_Selected(object sender, RoutedEventArgs e)
        {
            selectedItem = e.OriginalSource as TreeViewItem;
            (this.DataContext as Node).Owner.SelectedNode = TreeNodes.SelectedItem as Node;
            Node selectedNode = TreeNodes.SelectedItem as Node;

            if ((TreeNodes.DataContext as Node) == selectedNode.Owner.TabRoot.ChildNodes[2])
            {
                if (selectedNode.IsDir == true)
                {
                    BtnKeep.IsEnabled = false;
                }
                else
                {
                    BtnKeep.IsEnabled = true;
                }
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //R1.Height = new GridLength(26);
            //TreeNodes.BorderThickness = new Thickness(1);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //R1.Height = new GridLength(0);
            //TreeNodes.BorderThickness = new Thickness(1,0,1,1);
        }

        private void CommandBinding_CanExecute_AddFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.草稿.ToString() ||
            //    ((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.作品相关.ToString())
            //{
            //    e.CanExecute = false;
            //}
            //else
            //{
            //    e.CanExecute = true;
            //}
        }

        private void CommandBinding_CanExecute_ImportExport(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.草稿.ToString() ||
            //    ((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.作品相关.ToString() ||
            //    ((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.已发布.ToString() ||
            //    ((sender as TreeView).DataContext as Node).TabName == Material.MaterialTabName.范文.ToString() ||
            //    ((sender as TreeView).DataContext as Node).TabName == Material.MaterialTabName.资料.ToString())
            //{
            //    e.CanExecute = true;
            //}
            //else
            //{
            //    e.CanExecute = false;
            //}
        }

        private void CommandBinding_CanExecute_Keep(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.草稿.ToString())
            //{
            //    e.CanExecute = true;
            //}
            //else
            //{
            //    e.CanExecute = false;
            //}
        }

        private void CommandBinding_CanExecute_Send(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.草稿.ToString() ||
            //    ((sender as TreeView).DataContext as Node).TabName == Book.ChapterTabName.作品相关.ToString())
            //{
            //    e.CanExecute = true;
            //}
            //else
            //{
            //    e.CanExecute = false;
            //}
        }

        private void TreeNodes_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewAutomationPeer lvap = new TreeViewAutomationPeer(TreeNodes);
            var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
            if (svap != null)
            {
                var scroll = svap.Owner as ScrollViewer;

                if (Convert.ToBoolean(Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.Scroll2End)) == true)
                {
                    scroll.ScrollToEnd();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
            if ((this.DataContext as Node).TypeName == Book.TypeNameEnum.草稿.ToString())
            {
                BtnFolder.IsEnabled = false;
            }
            if ((this.DataContext as Node).TypeName == Book.TypeNameEnum.作品相关.ToString())
            {
                BtnFolder.IsEnabled = false;
                BtnKeep.IsEnabled = false;
            }
            if ((this.DataContext as Node).TypeName == Book.TypeNameEnum.已发布.ToString())
            {
                BtnSend.IsEnabled = false;
            }
            if (this.Tag != null && !this.Tag.Equals("Chapters"))
            {
                BtnKeep.Visibility = Visibility.Hidden;
                BtnSend.Visibility = Visibility.Hidden;
                BtnImport.Visibility = Visibility.Hidden;
                BtnExport.Visibility = Visibility.Hidden;
            }
        }

        private void TreeNodes_DragOver(object sender, DragEventArgs e)
        {

        }





        private void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            BtnClearSearch.Visibility = Visibility.Visible;
            foreach (Node node in (TreeNodes.DataContext as Node).GetHeirsList())
            {
                if (node.GetAllContent().Contains(e.Info.ToLower()))
                {
                    node.Visibility = Visibility.Visible;
                }
                else
                {
                    node.Visibility = Visibility.Collapsed;
                }
            }
            //把父节点也显示
            foreach (Node node in (TreeNodes.DataContext as Node).GetHeirsList())
            {
                if (node.Visibility == Visibility.Visible)
                {
                    node.Parent.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBar.Text))
            {
                return;
            }
            BtnClearSearch.Visibility = Visibility.Hidden;
            SearchBar.Clear();
            foreach (Node node in (TreeNodes.DataContext as Node).GetHeirsList())
            {
                node.Visibility = Visibility.Visible;
            }
        }

        private void SearchBar_MouseEnter(object sender, MouseEventArgs e)
        {
            SearchBar.Visibility = Visibility.Visible;
        }

        private void SearchBar_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void TreeNodes_MouseEnter(object sender, MouseEventArgs e)
        {
            SearchBar.Visibility = Visibility.Visible;
        }

        private void TreeNodes_MouseLeave(object sender, MouseEventArgs e)
        {
            SearchBar.Visibility = Visibility.Hidden;
        }


    }
}
