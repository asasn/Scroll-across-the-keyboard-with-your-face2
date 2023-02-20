﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using RootNS.Helper;
using RootNS.Models;

namespace RootNS.MyControls
{
    /// <summary>
    /// MyEditor.xaml 的交互逻辑
    /// </summary>
    public partial class Editorkernel : UserControl
    {
        public Editorkernel()
        {
            InitializeComponent();
            ThisTextEditor.TextArea.SelectionChanged += TextArea_SelectionChanged;
            ThisTextEditor.Document.Changing += Document_Changing;

            new System.Threading.Thread(SaveInThreadMethod);
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            Timer.Tick += TimeRuner;
            Timer.Start();

            theDialog = new FindReplaceDialog(this.ThisTextEditor);

        }

        private void ThisTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            ThisTextEditor.TextArea.Focus();
            //因为在TabControl中，每次切换的时候都会触发这个事件，故而一些初始化步骤放在父容器，不要放在这里

            if (ThisTextEditor.SyntaxHighlighting == null)
            {
                ThisTextEditor.SyntaxHighlighting = (this.DataContext as Node).Owner.Syntax;
            }
        }
        
        private bool canSaveFlag;
        Stopwatch stopWatch = new Stopwatch();
        public DispatcherTimer Timer = new DispatcherTimer();
        FindReplaceDialog theDialog;

        /// <summary>
        /// 方法：每次间隔运行的内容
        /// </summary>
        private void TimeRuner(object sender, EventArgs e)
        {
            if (BtnSaveDoc.IsEnabled == true)
            {
                if ((canSaveFlag == true || SysHelper.GetLastInputTime() >= 10 * 1000))
                {
                    SaveInThreadMethod(this.DataContext as Node);
                    stopWatch.Restart(); //保存时，重置stopWatch计时器
                }
            }
        }

        #region 命令
        private void Command_SaveText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                canSaveFlag = true;
                //Console.WriteLine(thread.ManagedThreadId + " - 成功 - " + thread.ThreadState);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(thread.ManagedThreadId + " - 失败 -" + thread.ThreadState);
                Console.WriteLine(string.Format("保存命令失败！\n{0}", ex));
            }
        }
        ///// <summary>
        ///// 在新线程中进行保存操作。
        ///// </summary>
        ///// <param name="node"></param>
        //private void SaveInThreadMethod(object node)
        //{
        //    SaveMethod(node as Node);
        //}

        private void SaveInThreadMethod(object para)
        {
            try
            {
                Node node = para as Node;
                node.Text = ThisTextEditor.Text;
                node.Count = textCount;
                BtnSaveDoc.IsEnabled = false;
                string sql = string.Format("UPDATE 内容 SET Text='{0}', Summary='{1}', Count='{2}' WHERE Guid='{3}';", node.Text.Replace("'", "''"), node.Summary.Replace("'", "''"), node.Count, node.Guid);
                SqliteHelper.PoolDict[node.Owner.Guid.ToString()].ExecuteNonQuery(sql);
                //保持连接会导致文件占用，不能及时同步和备份，过多重新连接则是不必要的开销。
                //故此在数据库占用和重复连接之间选择了一个平衡，允许保存之后的数据库得以上传。
                SqliteHelper.PoolDict[node.Owner.Guid.ToString()].Close();
                //HandyControl.Controls.Growl.SuccessGlobal(String.Format("{0} 已保存！", node.Title));
                Console.WriteLine(string.Format("本次保存成功！"));
                canSaveFlag = false;
            }
            catch (Exception ex)
            {
                //HandyControl.Controls.Growl.WarningGlobal(String.Format("本次保存失败！\n{0}", ex));
                Console.WriteLine(string.Format("本次保存失败！\n{0}", ex));
            }
        }


        private void Command_Typesetting_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditorHelper.TypeSetting(ThisTextEditor);
            Command_SaveText_Executed(null, null);
        }


        private void Command_Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplaceDialog.theDialog = FindReplaceDialog.ShowForReplace(ThisTextEditor);
            this.SetPreviousText();
            FindReplaceDialog.theDialog.TabFind.IsSelected = true;
            FindReplaceDialog.theDialog.TbFindNext.Focus();
        }

        private void Command_Replace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplaceDialog.theDialog = FindReplaceDialog.ShowForReplace(ThisTextEditor);
            this.SetPreviousText();
            FindReplaceDialog.theDialog.TabReplace.IsSelected = true;
            FindReplaceDialog.theDialog.txtFind2.Focus();
        }

        private void SetPreviousText()
        {
            if (string.IsNullOrEmpty(ThisTextEditor.TextArea.Selection.GetText()) == true)
            {
                if (string.IsNullOrEmpty(Gval.PreviousText) == true)
                {
                    //TODO:另行实现
                    //Gval.PreviousText = Gval.Views.UcSearch.TbKeyWords.Text;
                }
            }
            else
            {
                Gval.PreviousText = ThisTextEditor.TextArea.Selection.GetText();
            }
        }



        private void Command_MoveNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SetPreviousText();
            theDialog.cbSearchUp.IsChecked = false;
            theDialog.FindNext(Gval.PreviousText);
        }

        private void Command_MovePrevious_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SetPreviousText();
            theDialog.cbSearchUp.IsChecked = true;
            theDialog.FindNext(Gval.PreviousText);
        }

        private void Command_CloseTabItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Grid))
            {
                EditorHelper.CloseLightEditor(this, (((this.Parent as Grid).Parent as Control).Parent as Grid).Parent as Window);
            }

            if (this.Parent.GetType() == typeof(HandyControl.Controls.TabItem))
            {
               HandyControl.Controls.TabItem tabItem = this.Parent as HandyControl.Controls.TabItem;
               Workflow.FindByName(tabItem.CommandBindings, "Close").Execute(tabItem);
            }
        }

        private void Command_EditCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //TODO:编辑卡片
            //Card[] CardBoxs = { Gval.CurrentBook.CardRole, Gval.CurrentBook.CardOther, Gval.CurrentBook.CardWorld };
            //foreach (Card rootCard in CardBoxs)
            //{
            //    foreach (Card card in rootCard.ChildNodes)
            //    {
            //        if (ThisTextEditor.SelectedText.Equals(card.Title) == true || card.IsEqualsNickNames(ThisTextEditor.SelectedText, card.NickNames))
            //        {
            //            CardWindow cw = new CardWindow(card);
            //            cw.Left = ThisTextEditor.TranslatePoint(Mouse.GetPosition(this), Gval.View.MainWindow).X - 150;
            //            cw.Top = ThisTextEditor.TranslatePoint(Mouse.GetPosition(this), Gval.View.MainWindow).Y + 20;
            //            cw.ShowDialog();
            //            return;
            //        }
            //    }
            //}
        }
        #endregion

        #region 按钮点击事件


        public void BtnSaveText_Click(object sender, RoutedEventArgs e)
        {
            Command_SaveText_Executed(null, null);
        }

        private void BtnTypesetting_Click(object sender, RoutedEventArgs e)
        {
            Command_Typesetting_Executed(null, null);
        }

        /// <summary>
        /// 复制全文（可能存在格式化规则，具体看代码）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("　　\n");
            string nText = regex.Replace((this.DataContext as Node).Text, "");
            Clipboard.SetText(nText);
        }
        private void BtnCopyTitle_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((this.DataContext as Node).Title);
        }
        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            string temp = ThisTextEditor.Text;
            ThisTextEditor.Text = Clipboard.GetText();
            BtnUndo.DataContext = temp;
            EditorHelper.TypeSetting(ThisTextEditor);
            Command_SaveText_Executed(null, null);
            BtnUndo.IsEnabled = true;
        }
        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            ThisTextEditor.Text = BtnUndo.DataContext.ToString();
            EditorHelper.TypeSetting(ThisTextEditor);
            Command_SaveText_Executed(null, null);
            BtnUndo.IsEnabled = false;
        }

        private void ThisTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ThisTextEditor.TextArea.Document.Insert(ThisTextEditor.CaretOffset, "\n　　");
                ThisTextEditor.LineDown();
                Command_SaveText_Executed(null, null);
            }
            //逗号||句号的情况
            if (e.Key == Key.OemComma ||
                e.Key == Key.OemPeriod)
            {
                Command_SaveText_Executed(null, null);
            }
        }
        private void ThisTextEditor_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Command_CloseTabItem_Executed(null, null);
        }




        #endregion


        private void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            if (ThisTextEditor.SelectionLength > 0)
            {
                RefreshSelectionContent(textCount);
            }
            else
            {
                RefreshShowContent(textCount);
            }
        }



        private int textCount;
        private void Document_Changing(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            //更改过程，获得增删的文字
            textCount += EditorHelper.CountWords(e.InsertedText.Text);
            textCount -= EditorHelper.CountWords(e.RemovedText.Text);
        }

        private void ThisTextEditor_TextChanged(object sender, EventArgs e)
        {
            BtnSaveDoc.IsEnabled = true;
            //文字变更之后，刷新展示区
            RefreshShowContent(textCount);
        }

        /// <summary>
        /// 刷新字数统计和价值的显示
        /// </summary>
        /// <param name="textCount"></param>
        private void RefreshShowContent(int textCount)
        {
            LbWorksCount.Content = textCount;
            LbValueValue.Content = string.Format("{0:F}", Math.Round(Convert.ToDouble(textCount) * Gval.CurrentBook.Price / 1000, 2, MidpointRounding.AwayFromZero));
            LbValueValue5.Content = string.Format("{0:0}", Math.Floor((Convert.ToDouble(textCount) - 10) / 1000 * 5));
            LbValueValue4.Content = string.Format("{0:0}", Math.Floor((Convert.ToDouble(textCount) - 10) / 1000 * 4));
            LbValueValue3.Content = string.Format("{0:0}", Math.Floor((Convert.ToDouble(textCount) - 10) / 1000 * 3));
        }

        /// <summary>
        /// 刷新选中的字数统计和价值显示
        /// </summary>
        /// <param name="textCount"></param>
        private void RefreshSelectionContent(int textCount)
        {
            int sTextCount = EditorHelper.CountWords(ThisTextEditor.SelectedText);
            LbWorksCount.Content = string.Format("{0}/{1}", sTextCount, textCount);
            double sValue = Math.Round(Convert.ToDouble(sTextCount) * Gval.CurrentBook.Price / 1000, 2, MidpointRounding.AwayFromZero);
            double vValue = Math.Round(Convert.ToDouble(textCount) * Gval.CurrentBook.Price / 1000, 2, MidpointRounding.AwayFromZero);
            LbValueValue.Content = string.Format("{0:F}/{1:F}", sValue, vValue);
        }

        ToolTip toolTip = new ToolTip();

        /// <summary>
        /// 鼠标悬浮提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisTextEditor_MouseHover(object sender, MouseEventArgs e)
        {
           
        }

        /// <summary>
        /// 停止悬浮提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisTextEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            toolTip.IsOpen = false;
        }

        private void ThisTextEditor_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ThisTextEditor_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }


        private void TextEditorMenu_Opened(object sender, RoutedEventArgs e)
        {

        }

        bool isSliderLoaded = false;
        private void slider_Loaded(object sender, RoutedEventArgs e)
        {
            double sizePt = Convert.ToDouble(Settings.Get(Gval.CurrentBook, Gval.SettingsKeys.FontSizeBypt));
            if (sizePt != 0)
            {
                slider.Value = sizePt;
            }
            isSliderLoaded = true;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isSliderLoaded == true)
            {
                Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.FontSizeBypt, slider.Value);
            }
            ThisTextEditor.FontSize = (sender as Slider).Value * 16 / 12;
        }

        private void reSlider_Click(object sender, RoutedEventArgs e)
        {
            ThisTextEditor.FontSize = 16;
            slider.Value = 12;
        }


    }
}