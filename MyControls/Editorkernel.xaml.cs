﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using JiebaNet.Analyser;
using RootNS.Helper;
using RootNS.Models;
using RootNS.Views;

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
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
            ThisTextEditor.TextArea.SelectionChanged += TextArea_SelectionChanged;
            ThisTextEditor.Document.Changing += Document_Changing;

            new System.Threading.Thread(SaveInThreadMethod);
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            Timer.Tick += TimeRuner;
            Timer.Start();


            thisDialog = new FindReplaceDialog();
        }


        private FindReplaceDialog thisDialog;
        private bool? dialogTag;

        private void ThisTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
            //因为在TabControl中，每次切换的时候都会触发这个事件，故而一些初始化步骤放在父容器，不要放在这里
            if (ThisTextEditor.SyntaxHighlighting == null)
            {
                EditorHelper.UpdataSyntax();
                ThisTextEditor.FontFamily = new FontFamily(Convert.ToString(Settings.Get(Gval.CurrentBook, Gval.SettingsKeys.FontFamily)));
                if (string.IsNullOrEmpty(ThisTextEditor.FontFamily.Source) || this.Tag == null)
                {
                    //字体名字查无，或者是轻量编辑器时
                    ThisTextEditor.FontFamily = new FontFamily("宋体");
                }
            }
            if (this.Tag != null)
            {
                FindReplaceDialog.editor = ThisTextEditor;
                Gval.Views.UcShower.Tag = null;
                Gval.Views.UcShower.ThisTextEditor.Visibility = Visibility.Visible;
                Gval.Views.UcShower.DataContext = (this.DataContext as Node);
                Gval.Views.UcShower.ThisTextEditor.Text = (this.DataContext as Node).Summary;
                Gval.Views.UcShower.Tag = true;
            }
            if ((this.DataContext as Node).PointX == 0)
            {
                (this.DataContext as Node).PointX = (this.DataContext as Node).Text.Length;
            }
            ThisTextEditor.ScrollToLine(Math.Min(ThisTextEditor.LineCount, (int)(this.DataContext as Node).PointY));
            ThisTextEditor.Select(Math.Min(ThisTextEditor.Text.Length, (int)(this.DataContext as Node).PointX), 0);
            ThisTextEditor.TextArea.Focus();
        }


        private bool doSaveFlag;
        Stopwatch stopWatch = new Stopwatch();
        public DispatcherTimer Timer = new DispatcherTimer();

        /// <summary>
        /// 方法：每次间隔运行的内容
        /// </summary>
        private void TimeRuner(object sender, EventArgs e)
        {
            if (BtnSaveDoc.IsEnabled == true)
            {
                if ((doSaveFlag == true || SysHelper.GetLastInputTime() >= 10 * 1000))
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
                doSaveFlag = true;
                //Console.WriteLine(thread.ManagedThreadId + " - 成功 - " + thread.ThreadState);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("保存命令失败！\n{0}", ex));
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
                if (BtnSaveDoc.IsEnabled == false)
                {
                    return;
                }
                Node node = para as Node;
                node.Text = ThisTextEditor.Text;
                node.Summary = SummaryTextEditor.Text;
                node.PointX = ThisTextEditor.CaretOffset;
                ICSharpCode.AvalonEdit.Document.DocumentLine currentLine = ThisTextEditor.Document.GetLineByOffset(ThisTextEditor.CaretOffset);
                node.PointY = currentLine.LineNumber;
                this.DataContext = node;
                textCount = node.Count = CommonHelper.Count.QiDianCount(ThisTextEditor.Text);
                RefreshShowContent(textCount);
                string sql = string.Format("UPDATE 内容 SET Text='{0}', Summary='{1}', Title='{2}', Count='{3}', PointX={4}, PointY={5} WHERE Guid='{6}';", node.Text.Replace("'", "''"), node.Summary.Replace("'", "''"), node.Title.Replace("'", "''"), node.Count, node.PointX, node.PointY, node.Guid);
                SqliteHelper.PoolDict[node.Owner.Guid.ToString()].ExecuteNonQuery(sql);
                //保持连接会导致文件占用，不能及时同步和备份，过多重新连接则是不必要的开销。
                //故此在数据库占用和重复连接之间选择了一个平衡，允许保存之后的数据库得以上传。
                SqliteHelper.PoolDict[node.Owner.Guid.ToString()].Close();
                //HandyControl.Controls.Growl.SuccessGlobal(String.Format("{0} 已保存！", node.Title));
                Console.WriteLine(string.Format("本次保存成功！"));
                doSaveFlag = false;
                BtnSaveDoc.IsEnabled = false;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("本次保存失败！\n{0}", ex));
            }
        }


        private void Command_Typesetting_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditorHelper.TypeSetting(this.ThisTextEditor);
            Command_SaveText_Executed(null, null);
        }


        private void Command_Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Gval.Views.UcFindReplaceDialog = FindReplaceDialog.ShowForReplace();
            dialogTag = false;
            this.SetPreviousText();
            Gval.Views.UcFindReplaceDialog.TabFind.IsSelected = true;
            Gval.Views.UcFindReplaceDialog.txtFind.SelectAll();
            Gval.Views.UcFindReplaceDialog.txtFind.Focus();
        }

        private void Command_Replace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Gval.Views.UcFindReplaceDialog = FindReplaceDialog.ShowForReplace();
            dialogTag = true;
            this.SetPreviousText();
            Gval.Views.UcFindReplaceDialog.TabReplace.IsSelected = true;
            Gval.Views.UcFindReplaceDialog.txtFind2.SelectAll();
            Gval.Views.UcFindReplaceDialog.txtFind2.Focus();
        }

        private void SetPreviousText()
        {
            if (string.IsNullOrEmpty(this.ThisTextEditor.TextArea.Selection.GetText()) == true)
            {
                if (string.IsNullOrEmpty(Gval.Views.UcSearcher.TbKeyWords.Text))
                {

                }
                else
                {
                    Gval.PreviousText = Gval.Views.UcSearcher.TbKeyWords.Text;
                }

            }
            else
            {
                Gval.PreviousText = this.ThisTextEditor.TextArea.Selection.GetText();
            }
        }



        private void Command_MoveNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SetPreviousText();
            thisDialog.cbSearchUp.IsChecked = false;
            thisDialog.FindNext(Gval.PreviousText);
        }

        private void Command_MovePrevious_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SetPreviousText();
            thisDialog.cbSearchUp.IsChecked = true;
            thisDialog.FindNext(Gval.PreviousText);
        }

        private void Command_CloseTabItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(HandyControl.Controls.TabItem))
            {
                HandyControl.Controls.TabItem tabItem = this.Parent as HandyControl.Controls.TabItem;
                Workflow.FindByName(tabItem.CommandBindings, "Close").Execute(tabItem);
            }
        }

        private void Command_EditCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool isMatch = false;
            foreach (Node node in (this.DataContext as Node).Owner.TabRoot.ChildNodes[5].GetHeirsList())
            {
                if (node.Attachment == null || node.Card == null ||
                    node.IsDel == true || node.IsDir == true ||
                    string.IsNullOrEmpty(node.Title.Trim()))
                {
                    continue;
                }
                if (this.ThisTextEditor.SelectedText.Equals(node.Title.Trim()))
                {
                    isMatch = true;
                }
                else
                {
                    foreach (Card.Line.Tip tip in node.Card.Lines[0].Tips)
                    {
                        if (this.ThisTextEditor.SelectedText.Equals(tip.Content.Trim()))
                        {
                            isMatch = true;
                        }
                    }
                }
                if (isMatch)
                {
                    //匹配的情况
                    //在生成窗口对象之后，展现界面之前赋值给DataContext
                    WCard wCard = new WCard();
                    wCard.DataContext = node;
                    wCard.Show();
                    return;
                }
            }
            if (isMatch == false && !string.IsNullOrWhiteSpace(this.ThisTextEditor.SelectedText))
            {
                //未匹配的情况
                WCard wCard = new WCard();
                Node newNode = new Node() { Title = this.ThisTextEditor.SelectedText };
                Gval.CurrentBook.TabRoot.ChildNodes[5].ChildNodes.Add(newNode);
                wCard.DataContext = newNode;
                wCard.Show();
                newNode.Card.HasChange = true; //需要添加进去之后才能生成卡片，以及Show之后再扳回True，所以，注意这条语句要在Add和Show之后；
                return;
            }
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
            string temp = this.ThisTextEditor.Text;
            this.ThisTextEditor.Text = Clipboard.GetText();
            BtnUndo.DataContext = temp;
            EditorHelper.TypeSetting(this.ThisTextEditor);
            Command_SaveText_Executed(null, null);
            BtnUndo.IsEnabled = true;
        }
        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            this.ThisTextEditor.Text = BtnUndo.DataContext.ToString();
            EditorHelper.TypeSetting(this.ThisTextEditor);
            Command_SaveText_Executed(null, null);
            BtnUndo.IsEnabled = false;
        }

        private void ThisTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ThisTextEditor.TextArea.Document.Insert(this.ThisTextEditor.CaretOffset, "\n　　");
                this.ThisTextEditor.LineDown();
                Command_SaveText_Executed(null, null);
            }
            //逗号||句号的情况
            if (e.Key == Key.OemComma ||
                e.Key == Key.OemPeriod)
            {
                Command_SaveText_Executed(null, null);
            }
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
            textCount += CommonHelper.Count.QiDianCount(e.InsertedText.Text);
            textCount -= CommonHelper.Count.QiDianCount(e.RemovedText.Text);
        }

        private void ThisTextEditor_TextChanged(object sender, EventArgs e)
        {
            BtnSaveDoc.IsEnabled = true;
            //文字变更之后，刷新展示区
            RefreshShowContent(textCount);
        }
        private void SummaryTextEditor_TextChanged(object sender, EventArgs e)
        {
            if (this.Tag != null)
            {
                //不是轻量编辑器时
                if (SummaryTextEditor.Text.Equals((this.DataContext as Node).Summary))
                {

                }
                else
                {
                    BtnSaveDoc.IsEnabled = true;
                }
                ////文字变更之后，刷新展示区
                //Gval.Views.UcShower.ThisTextEditor.Text = SummaryTextEditor.Text;
                //ICSharpCode.AvalonEdit.Document.DocumentLine currentLine = Gval.Views.UcShower.ThisTextEditor.Document.GetLineByOffset(SummaryTextEditor.CaretOffset);
                //Gval.Views.UcShower.ThisTextEditor.ScrollToLine(currentLine.LineNumber);
            }
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
            int sTextCount = CommonHelper.Count.QiDianCount(ThisTextEditor.SelectedText);
            LbWorksCount.Content = string.Format("{0}/{1}", sTextCount, textCount);
            double sValue = Math.Round(Convert.ToDouble(sTextCount) * Gval.CurrentBook.Price / 1000, 2, MidpointRounding.AwayFromZero);
            double vValue = Math.Round(Convert.ToDouble(textCount) * Gval.CurrentBook.Price / 1000, 2, MidpointRounding.AwayFromZero);
            LbValueValue.Content = string.Format("{0:F}/{1:F}", sValue, vValue);
        }

        ToolTip toolTip = new ToolTip() { Background = Brushes.Transparent, BorderBrush = Brushes.Transparent };

        /// <summary>
        /// 鼠标悬浮提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisTextEditor_MouseHover(object sender, MouseEventArgs e)
        {
            if (ThisTextEditor.SyntaxHighlighting == null)
            {
                return;
            }
            TextViewPosition? pos = ThisTextEditor.GetPositionFromPoint(e.GetPosition(ThisTextEditor));
            if (pos != null)
            {
                int offset = ThisTextEditor.Document.GetOffset(pos.Value.Location);
                foreach (HighlightingRule rule in ThisTextEditor.SyntaxHighlighting.MainRuleSet.Rules)
                {
                    ICSharpCode.AvalonEdit.Document.DocumentLine line = ThisTextEditor.Document.GetLineByOffset(offset);
                    string segment = ThisTextEditor.Document.GetText(line);
                    int lineOffset = offset - line.Offset;
                    System.Text.RegularExpressions.MatchCollection matches = rule.Regex.Matches(segment);
                    if (matches.Count > 0)
                    {
                        foreach (System.Text.RegularExpressions.Match match in matches)
                        {
                            if (match.Index <= lineOffset && lineOffset - match.Index <= match.Value.Length)
                            {
                                Node card = Workflow.GetMatchCard(match, (this.DataContext as Node).Owner);
                                if (card != null)
                                {
                                    toolTip.Content = new CardShower(card);
                                    toolTip.IsOpen = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
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

        bool isSlider1Loaded = false;
        private void slider1_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag == null)
            {
                //是轻量编辑器时
                return;
            }
            double size = Convert.ToDouble(Settings.Get(Gval.CurrentBook, Gval.SettingsKeys.TextAreaMargin));
            if (size != 0)
            {
                slider1.Value = size;
            }
            isSlider1Loaded = true;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isSlider1Loaded == true)
            {
                Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.TextAreaMargin, slider1.Value);
            }
            double aaa = (sender as Slider).Value * 10;
            ThisTextEditor.TextArea.Margin = new Thickness(aaa, 0, aaa, 0);
        }

        bool isSlider2Loaded = false;
        private void slider2_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag == null)
            {
                //是轻量编辑器时
                return;
            }
            double size = Convert.ToDouble(Settings.Get(Gval.CurrentBook, Gval.SettingsKeys.FontSize));
            if (size != 0)
            {
                slider2.Value = size;
            }
            isSlider2Loaded = true;
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isSlider2Loaded == true)
            {
                Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.FontSize, slider2.Value);
            }
            ThisTextEditor.FontSize = (sender as Slider).Value;
        }

        bool _isReadModel = false;
        private void readModel_Click(object sender, RoutedEventArgs e)
        {
            if (ThisTextEditor.FontFamily.Source == "霞鹜文楷")
            {
                _isReadModel = true;
            }
            if (_isReadModel)
            {
                ThisTextEditor.TextArea.Margin = new Thickness(0);
                slider1.Value = 0;
                slider2.Value = 16;
                CbShowLineNumbers.IsChecked = true;
                ThisTextEditor.FontFamily = new FontFamily("宋体");
                Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.FontFamily, ThisTextEditor.FontFamily);
                _isReadModel = false;
            }
            else
            {
                slider1.Value = 15;
                slider2.Value = 24;
                CbShowLineNumbers.IsChecked = false;
                ThisTextEditor.FontFamily = new FontFamily("霞鹜文楷");
                Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.FontFamily, ThisTextEditor.FontFamily);
                _isReadModel = true;
            }

        }

        private void reSlider_Click(object sender, RoutedEventArgs e)
        {
            ThisTextEditor.TextArea.Margin = new Thickness(0);
            slider1.Value = 0;
            slider2.Value = 16;
            CbShowLineNumbers.IsChecked = true;
            ThisTextEditor.FontFamily = new FontFamily("宋体");
            Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.FontFamily, ThisTextEditor.FontFamily);
        }

        bool isCbShowLineNumbersLoad = false;
        private void CbShowLineNumbers_Loaded(object sender, RoutedEventArgs e)
        {
            CbShowLineNumbers.IsChecked = Convert.ToBoolean(Settings.Get(Gval.CurrentBook, Gval.SettingsKeys.IsShowLineNumbers));
            isCbShowLineNumbersLoad = true;
        }

        private void isCbShowLineNumbers_Change(object sender, RoutedEventArgs e)
        {
            if (isCbShowLineNumbersLoad == true)
            {
                Settings.Set(Gval.CurrentBook, Gval.SettingsKeys.IsShowLineNumbers, CbShowLineNumbers.IsChecked);
            }
            ThisTextEditor.ShowLineNumbers = (bool)(sender as ToggleButton).IsChecked;
        }


        private void BtnSummary_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSummary.Tag == null)
            {
                SwitchToSummaryTextEditor();
            }
            else
            {
                SwitchToThisTextEditor();
            }
        }

        private void SwitchToSummaryTextEditor()
        {
            Row01.Height = new GridLength(15);
            Row02.Height = new GridLength(400);
            Row02.MinHeight = 300;
            BtnSummary.Tag = true;
            SummaryTextEditor.ScrollToEnd();
            SummaryTextEditor.Select(SummaryTextEditor.Text.Length, 0);
            SummaryTextEditor.TextArea.Focus();
        }


        private void SwitchToThisTextEditor()
        {
            Row02.MinHeight = 0;
            Row01.Height = new GridLength(0);
            Row02.Height = new GridLength(0);
            BtnSummary.Tag = null;
            ThisTextEditor.TextArea.Focus();
        }


        private void MenuItem0_Click(object sender, RoutedEventArgs e)
        {
            FunctionsPack.ShowMessageBox("新增信息卡（施工中）！");
        }

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            //把选中的文字加入文章片段的功能
            if (string.IsNullOrEmpty(ThisTextEditor.SelectedText))
            {
                return;
            }
            var extractor = new TfidfExtractor();
            IEnumerable strs = extractor.ExtractTags(ThisTextEditor.SelectedText, 5, null);
            string title = string.Empty;
            foreach (string s in strs)
            {
                title += s + " ";
            }
            Node newNode = new Node
            {
                Title = title.Trim(),
                Text = ThisTextEditor.SelectedText,
                Count = CommonHelper.Count.QiDianCount(ThisTextEditor.SelectedText),
            };
            (this.DataContext as Node).Owner.TabRoot.ChildNodes[4].GetLastNode(true).ChildNodes.Add(newNode);
            newNode.Insert();
            ThisTextEditor.SelectedText = "";
        }


        private void ThisTextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ThisTextEditor = (TextEditor)sender;
            SwitchToThisTextEditor();
        }

        private void SummaryTextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ThisTextEditor = (TextEditor)sender;
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            ICSharpCode.AvalonEdit.Document.DocumentLine currentLine = ThisTextEditor.Document.GetLineByOffset(ThisTextEditor.CaretOffset);
            ThisTextEditor.ScrollToLine(currentLine.LineNumber);
        }


    }
}
