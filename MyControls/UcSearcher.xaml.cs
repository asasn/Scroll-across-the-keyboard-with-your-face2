using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace RootNS.MyControls
{
    /// <summary>
    /// uc_Searcher.xaml 的交互逻辑
    /// </summary>
    public partial class UcSearcher : UserControl
    {
        public UcSearcher()
        {
            InitializeComponent();
        }

        public Searcher ThisSearcher { get; set; } = new Searcher();

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ThisSearcher.Start();
        }


        private void TbKeyWords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxOfResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Searcher.Item lbItem = (Searcher.Item)ListBoxOfResults.SelectedItem;
            //WSearchResult rtWin = new WSearchResult(lbItem);
            //rtWin.ShowDialog();
        }

        private void ThisControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CbMaterial_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (CbMaterial.IsChecked == true)
            {
                CbChapters.Content = "资料"; CbEvents.Content = "情节"; CbSnippets.Content = "片段"; CbCards.Content = "卡片";
            }
            else
            {
                CbChapters.Content = "章节"; CbEvents.Content = "事件"; CbSnippets.Content = "片段"; CbCards.Content = "卡片";
            }
        }

        private void CbTitle_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (CbTitle.IsChecked == null)
            {
                CbTitle.Content = "标题内容全搜";
                return;
            }
            if (CbTitle.IsChecked == true)
            {
                CbTitle.Content = "当前只搜标题";
            }
            else
            {
                CbTitle.Content = "当前只搜内容";
            }
        }

        private void ListBoxMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (ListBoxOfResults.SelectedItem == null)
            {
                return;
            }
            ((sender as ContextMenu).Items[0] as MenuItem).IsEnabled = true;
            ((sender as ContextMenu).Items[1] as MenuItem).IsEnabled = true;
        }

        private void Command_Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((ListBoxOfResults.SelectedItem as Node) == null)
            {
                return;
            }
            Node node = ListBoxOfResults.SelectedItem as Node;
            node.Remove();
            BtnSearch_Click(null, null);
        }

        private void Command_UnDel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((ListBoxOfResults.SelectedItem as Node) == null)
            {
                return;
            }
            Node node = ListBoxOfResults.SelectedItem as Node;
            if (node.IsDel == true)
            {
                node.UnDel();
            }
        }
    }
}
