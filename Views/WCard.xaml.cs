﻿using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections;
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
using System.Windows.Threading;

namespace RootNS.Views
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class WCard : Window
    {
        public WCard()
        {
            InitializeComponent();

        }

        public Node OldParent { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as Node).Attachment == null)
            {
                (this.DataContext as Node).Attachment = string.Empty;
            }
            TbTitle.Text = (this.DataContext as Node).Title;
            TbSummary.Text = (this.DataContext as Node).Summary;
            TbBornYear.Text = (this.DataContext as Node).PointX.ToString();
            TbAge.Text = ((this.DataContext as Node).Owner.CurrentYear - Convert.ToInt64(TbBornYear.Text)).ToString();
            if (string.IsNullOrEmpty((this.DataContext as Node).Card.Tag))
            {
                (this.DataContext as Node).Card.Tag = "未指定";
            }
            TbTag.SelectedItem = (this.DataContext as Node).Parent;
            (this.DataContext as Node).PointY = Convert.ToInt64(TbAge.Text);
            (this.DataContext as Node).Card.HasChange = false;
            //获取鼠标位置以设置窗口
            Point point = Mouse.GetPosition(Gval.Views.MainWindow);
            this.Left = point.X - this.ActualWidth * 0.5;
            this.Top = point.Y - 26;
            BtnSeeMore_Click(null, null);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //左键按下时，允许拖曳
            DragMove();
        }

        /// <summary>
        /// 更新信息卡
        /// </summary>
        private void UpdataCard()
        {
            //ToDo：这里可以进行一些sql语句的优化
            (this.DataContext as Node).Card.Tag = (this.DataContext as Node).Parent.Title;
            (this.DataContext as Node).Title = TbTitle.Text;
            (this.DataContext as Node).PointX = Convert.ToInt64(TbBornYear.Text);
            (this.DataContext as Node).PointY = Convert.ToInt64(TbAge.Text);
            (this.DataContext as Node).Summary = TbSummary.Text;
            (this.DataContext as Node).Attachment = JsonHelper.Otj<RootNS.Models.Card>((this.DataContext as Node).Card);
            (this.DataContext as Node).UpdateNodeProperty("内容", "Title", (this.DataContext as Node).Title);
            (this.DataContext as Node).UpdateNodeProperty("内容", "PointX", (this.DataContext as Node).PointX.ToString());
            (this.DataContext as Node).UpdateNodeProperty("内容", "PointY", (this.DataContext as Node).PointY.ToString());
            (this.DataContext as Node).UpdateNodeProperty("内容", "Summary", (this.DataContext as Node).Summary);
            (this.DataContext as Node).UpdateNodeProperty("内容", "Attachment", (this.DataContext as Node).Attachment.ToString());
            (this.DataContext as Node).UpdateNodeProperty("节点", "Puid", (this.DataContext as Node).Parent.Guid.ToString());
            (this.DataContext as Node).UpdateNodeProperty("节点", "Index", (this.DataContext as Node).Index.ToString());

            EditorHelper.UpdataSyntax();
        }




        /// <summary>
        /// 是否存在同名节点
        /// </summary>
        /// <returns></returns>
        private bool HasDuplicateTitle()
        {
            foreach (Node node in (this.DataContext as Node).Parent.ChildNodes)
            {
                //标题相同，但是Guid不同时
                if (TbTitle.Text.Trim() == node.Title &&
                    node.Guid != (this.DataContext as Node).Guid && node.Title != "新节点")
                {
                    FunctionsPack.ShowMessageBox("存在同名节点，请使用其他名称！");
                    return true;
                }
            }
            return false;
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (HasDuplicateTitle())
            {
                return;
            }
            (this.DataContext as Node).Card.ClaerNullTips();
            (this.DataContext as Node).Card.HasChange = false;
            if ((this.DataContext as Node).HasSave == false)
            {
                (this.DataContext as Node).Insert();
            }
            UpdataCard();
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
                    bool tag = false;
                    foreach (Card.Line line in (this.DataContext as Node).Card.Lines)
                    {
                        foreach (Card.Line.Tip tip in line.Tips)
                        {
                            if (string.IsNullOrEmpty(tip.Content) == false)
                            {
                                tag = true;
                                break;
                            }
                        }
                    }
                    if (tag == false)
                    {
                        (this.DataContext as Node).Parent.ChildNodes.Remove(this.DataContext as Node);
                    }
                    if (OldParent != null)
                    {
                        OldParent.ChildNodes.Add(this.DataContext as Node);
                    }
                    EditorHelper.UpdataSyntax();
                }
                if (dr == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            (this.DataContext as Node).Card.HiddenNullLines();
            this.Close();
        }


        private void BtnSeeLess_Click(object sender, RoutedEventArgs e)
        {
            //从全模式转变为隐藏模式
            BtnSeeLess.Visibility = Visibility.Hidden;
            BtnSeeMore.Visibility = Visibility.Visible;
            (this.DataContext as Node).Card.HiddenNullLines();
        }

        private void BtnSeeMore_Click(object sender, RoutedEventArgs e)
        {
            //从隐藏模式转变为全模式
            BtnSeeLess.Visibility = Visibility.Visible;
            BtnSeeMore.Visibility = Visibility.Hidden;
            foreach (Card.Line line in (this.DataContext as Node).Card.Lines)
            {
                line.Visibility = true;
            }
        }

        private void TbTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).IsEnabled == false)
            {
                TbTag.IsEnabled = true;
                return;
            }
            if (TbTag.SelectedItem == null)
            {
                return;
            }
            OldParent = (this.DataContext as Node).Parent;
            (TbTag.SelectedItem as Node).ChildNodes.Add(this.DataContext as Node);
            OldParent.ChildNodes.Remove(this.DataContext as Node);
            (this.DataContext as Node).Card.HasChange = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.DataContext as Node).Card.HasChange = true;
        }
        private void TbBornYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.DataContext as Node).Card.HasChange = true;
            TbBornYear.Text = InputCheck(TbBornYear.Text);
            TbAge.Text = ((this.DataContext as Node).Owner.CurrentYear - Convert.ToInt64(TbBornYear.Text)).ToString();
        }

        private string InputCheck(string strInput)
        {
            long.TryParse(strInput, out long longOut);
            return longOut.ToString();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Card.Line.Tip newTip = new Card.Line.Tip();
            ((e.Source as Button).DataContext as Card.Line).Tips.Add(newTip);
            _tipSwitch = true;
        }

        /// <summary>
        /// Tip生成的TextBox控件，焦点开关
        /// </summary>
        private bool _tipSwitch = false;

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tipSwitch == true)
            {
                (sender as TextBox).Focus();
                _tipSwitch = false;
            }
        }
    }
}
