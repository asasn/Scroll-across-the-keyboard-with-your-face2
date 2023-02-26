using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RootNS.MyControls
{
    public partial class MyTree : UserControl
    {
        #region 命令
        Node _lastReNameNode;
        private void Command_ReName_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = _lastReNameNode = TreeNodes.SelectedItem as Node;
            if (selectedNode != null && selectedNode.IsChecked == false)
            {
                selectedNode.ReNameing = !selectedNode.ReNameing;
                TextBox TbReName = ControlHelper.FindChild<TextBox>(selectedItem as DependencyObject, "TbReName");
                TbReName.SelectAll();
                TbReName.Focus();
            }
        }

        private void Command_AddFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            Node rootNode = TreeNodes.DataContext as Node;
            Node node = new Node() { IsDir = true };
            rootNode.ChildNodes.Add(node);
            node.Insert();
        }

        private void Command_AddDoc_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            Node rootNode = TreeNodes.DataContext as Node;
            Node node = new Node();
            if (selectedNode == null)
            {
                rootNode.ChildNodes.Add(node);
            }
            else
            {
                if (selectedNode.IsDir == true)
                {
                    selectedNode.ChildNodes.Add(node);
                }
                else
                {
                    selectedNode.Parent.ChildNodes.Add(node);
                }
            }
            node.Insert();
        }
        private void Command_InsertDoc_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            Node rootNode = TreeNodes.DataContext as Node;
            Node node = new Node();
            if (selectedNode == null)
            {
                rootNode.ChildNodes.Add(node);
            }
            else
            {
                if (selectedNode.IsDir == true)
                {
                    selectedNode.ChildNodes.Add(node);
                }
                else
                {
                    int index = selectedNode.Parent.ChildNodes.IndexOf(selectedNode);
                    selectedNode.Parent.ChildNodes.Insert(index, node);
                    node.ChangeBrothersIndex(index);
                }
            }
            node.Insert();
        }

        private void Command_Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((TreeNodes.SelectedItem as Node) == null)
            {
                return;
            }
            Node node = TreeNodes.SelectedItem as Node;
            node.Remove();
        }

        private void Command_UnDel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((TreeNodes.SelectedItem as Node) == null)
            {
                return;
            }
            Node node = TreeNodes.SelectedItem as Node;
            if (node.IsDel == true)
            {
                node.UnDel();
            }
        }

        private void Command_MoveUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TreeNodes.SelectedItem == null)
            {
                return;
            }
            (TreeNodes.SelectedItem as Node).MoveUp();
        }

        private void Command_MoveDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TreeNodes.SelectedItem == null)
            {
                return;
            }
            (TreeNodes.SelectedItem as Node).MoveDown();
        }

        private void Command_Keep_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            if (selectedNode == null)
            {
                return;
            }
            Node dstNode = selectedNode.Owner.TabRoot.ChildNodes[1];
            if (dstNode.ChildNodes.Count > 0 && dstNode.ChildNodes.Last<Node>().IsDir == true)
            {
                selectedNode.MoveTo(dstNode.ChildNodes.Last<Node>());
            }
            else
            {
                selectedNode.MoveTo(dstNode);
            }

        }
        private void Command_Send_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            if (selectedNode == null)
            {
                return;
            }
            Node pNode = selectedNode.Owner.TabRoot.ChildNodes[2];
            Node finalDir = pNode.GetLastNode();
            selectedNode.MoveTo(finalDir);
        }



        private void Command_Import_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            if (selectedNode == null)
            {
                selectedNode = (sender as Button).DataContext as Node;
            }
            selectedNode.Import();
        }
        private void Command_Export_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node selectedNode = TreeNodes.SelectedItem as Node;
            if (selectedNode == null)
            {
                selectedNode = (sender as Button).DataContext as Node;
            }
            selectedNode.Export();
        }

        private void Command_CopyTitle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText((TreeNodes.SelectedItem as Node).Title);
            HandyControl.Controls.Growl.SuccessGlobal("已复制本节点标题到剪贴板！");
        }

        private void Command_CopyText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Regex regex = new Regex("　　\n");
            string nText = regex.Replace((TreeNodes.SelectedItem as Node).Text, "");
            Clipboard.SetText(nText);
            HandyControl.Controls.Growl.SuccessGlobal("已复制本节点文本到剪贴板！\n（不包含未保存的部分）");
        }


        #endregion
    }
}
