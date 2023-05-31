using System;
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

            theDialog = new FindReplaceDialog(this.ThisTextEditor);

        }

        private void ThisTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
            Gval.Views.CurrentEditorkernel = this;
            ThisTextEditor.TextArea.Focus();
            //因为在TabControl中，每次切换的时候都会触发这个事件，故而一些初始化步骤放在父容器，不要放在这里

            if (ThisTextEditor.SyntaxHighlighting == null)
            {
                (this.DataContext as Node).Owner.UpdataSyntax();
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
                Node node = para as Node;
                node.Text = ThisTextEditor.Text;
                textCount = node.Count = CommonHelper.Count.QiDianCount(ThisTextEditor.Text);
                RefreshShowContent(textCount);
                string sql = string.Format("UPDATE 内容 SET Text='{0}', Summary='{1}', Title='{2}', Count='{3}' WHERE Guid='{4}';", node.Text.Replace("'", "''"), node.Summary.Replace("'", "''"), node.Title.Replace("'", "''"), node.Count, node.Guid);
                SqliteHelper.PoolDict[node.Owner.Guid.ToString()].ExecuteNonQuery(sql);
                //保持连接会导致文件占用，不能及时同步和备份，过多重新连接则是不必要的开销。
                //故此在数据库占用和重复连接之间选择了一个平衡，允许保存之后的数据库得以上传。
                SqliteHelper.PoolDict[node.Owner.Guid.ToString()].Close();
                //HandyControl.Controls.Growl.SuccessGlobal(String.Format("{0} 已保存！", node.Title));
                Console.WriteLine(string.Format("本次保存成功！"));
                canSaveFlag = false;
                BtnSaveDoc.IsEnabled = false;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("本次保存失败！\n{0}", ex));
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
            FindReplaceDialog.theDialog.txtFind.SelectAll();
            FindReplaceDialog.theDialog.txtFind.Focus();
        }

        private void Command_Replace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplaceDialog.theDialog = FindReplaceDialog.ShowForReplace(ThisTextEditor);
            this.SetPreviousText();
            FindReplaceDialog.theDialog.TabReplace.IsSelected = true;
            FindReplaceDialog.theDialog.txtFind2.SelectAll();
            FindReplaceDialog.theDialog.txtFind2.Focus();
        }

        private void SetPreviousText()
        {
            if (string.IsNullOrEmpty(ThisTextEditor.TextArea.Selection.GetText()) == true)
            {
                Gval.PreviousText = Gval.Views.UcSearcher.TbKeyWords.Text;

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
                    string.IsNullOrEmpty(node.Title.Trim()) )
                {
                    continue;
                }
                if (ThisTextEditor.SelectedText.Equals(node.Title.Trim()))
                {
                    isMatch = true;
                }
                else
                {
                    foreach (Card.Line.Tip tip in node.Card.Lines[0].Tips)
                    {
                        if (ThisTextEditor.SelectedText.Equals(tip.Content.Trim()))
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
            if (isMatch == false && !string.IsNullOrWhiteSpace(ThisTextEditor.SelectedText))
            {
                //未匹配的情况
                WCard wCard = new WCard();
                Node newNode = new Node() { Title = ThisTextEditor.SelectedText};
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
            textCount += CommonHelper.Count.QiDianCount(e.InsertedText.Text);
            textCount -= CommonHelper.Count.QiDianCount(e.RemovedText.Text);
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
                title += s  + " ";
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
    }
}
