using RootNS.Helper;
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as Node).Attachment == null)
            {
                (this.DataContext as Node).Attachment = string.Empty;
            }
            ThisCard = JsonHelper.Jto<Card>((this.DataContext as Node).Attachment.ToString());
            if (ThisCard == null)
            {
                ThisCard = new Card();
            }
            HiddenNullLines();
        }

        public Visibility LineVisibility { get; set; } = Visibility.Visible;

        public Card ThisCard
        {
            get { return (Card)GetValue(ThisCardProperty); }
            set { SetValue(ThisCardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThisCard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThisCardProperty =
            DependencyProperty.Register("ThisCard", typeof(Card), typeof(WCard), new PropertyMetadata(new Card()));


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //左键按下时，允许拖曳
            DragMove();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ClaerNullLines();
            (this.DataContext as Node).HasChange = false;
            (this.DataContext as Node).Attachment = ThisCard;
            (this.DataContext as Node).UpdateNodeProperty("内容", "Title", (this.DataContext as Node).Title);
            (this.DataContext as Node).UpdateNodeProperty("内容", "PointX", (this.DataContext as Node).PointX.ToString());
            (this.DataContext as Node).UpdateNodeProperty("内容", "Summary", (this.DataContext as Node).Summary);
            (this.DataContext as Node).UpdateNodeProperty("内容", "Attachment", JsonHelper.Otj(ThisCard));
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 清除空行
        /// </summary>
        private void ClaerNullLines()
        {
            foreach (Card.Line line in ThisCard.Lines)
            {
                //当移除完元素之后，数组大小发生了变更，会抛出异常，所以在这里使用逆序遍历来进行
                for (int i = line.Tips.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(line.Tips[i].Content))
                    {
                        line.Tips.Remove(line.Tips[i]);
                    }
                }
            }
        }


        /// <summary>
        /// 隐藏一部分行
        /// </summary>
        private void HiddenNullLines()
        {
            foreach (Card.Line line in ThisCard.Lines)
            {
                bool allNull = true;//假设全部为空，只要有一个反例就可以跳出循环
                foreach (Card.Line.Tip tip in line.Tips)
                {
                    if (string.IsNullOrEmpty(tip.Content) == false)
                    {
                        allNull = false;
                        break;
                    }
                }
                if (allNull)
                {
                    line.Visibility = false;
                }
                else
                {
                    line.Visibility = true;
                }
            }
        }


        private void BtnSeeMore_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSeeMore.Visibility == Visibility.Visible)
            {
                //从全模式转变为隐藏模式
                BtnSeeMore.Visibility = Visibility.Hidden;
                BtnSeeMore2.Visibility = Visibility.Visible;
                HiddenNullLines();
            }
            else
            {
                //从隐藏模式转变为全模式
                BtnSeeMore.Visibility = Visibility.Visible;
                BtnSeeMore2.Visibility = Visibility.Hidden;
                foreach (Card.Line line in ThisCard.Lines)
                {
                    line.Visibility = true;
                }

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.DataContext as Node).HasChange = true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ((e.Source as Button).DataContext as Card.Line).Tips.Add(new Card.Line.Tip());
        }


    }
}
