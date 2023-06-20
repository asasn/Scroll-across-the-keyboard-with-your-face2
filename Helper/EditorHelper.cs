using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using RootNS.Models;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace RootNS.Helper
{
    internal class EditorHelper
    {
        /// <summary>
        /// 语法高亮对象
        /// </summary>
        private static IHighlightingDefinition _syntax;


        private static HighlightingRule NewRule(string keyword, string colorTag)
        {
            try
            {
                new Regex(keyword);
            }
            catch (Exception ex)
            {
                keyword = "\\" + keyword;
                throw new Exception(string.Format("高亮规则添加关键词时发生错误！\n{0}", ex));
            }
            HighlightingRule rule = new HighlightingRule
            {
                Color = _syntax.GetNamedColor(colorTag),
                Regex = new Regex(keyword.Trim())
            };
            return rule;
        }

        /// <summary>
        /// 列表内部的元素按照长度排序方法（注意正反排序，调整xy的顺序实现）
        /// </summary>
        private class StringLengthComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return (((string, string))y).Item1.Length.CompareTo((((string, string))x).Item1.Length);
            }
        }

        /// <summary>
        /// 刷新配色方案，填入本控件语法对象
        /// </summary>
        public static void UpdataSyntax()
        {
            System.Xml.XmlTextReader xshdReader = new System.Xml.XmlTextReader(Gval.Path.XshdFilePath);
            _syntax = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xshdReader, HighlightingManager.Instance);
            xshdReader.Close();
            System.Collections.ArrayList NameArrayList = new System.Collections.ArrayList();
            System.Collections.ArrayList nodes = new System.Collections.ArrayList();
            nodes.AddRange(Gval.CurrentBook.TabRoot.ChildNodes[5].GetHeirsList());
            nodes.AddRange(Gval.MaterialBook.TabRoot.ChildNodes[8].GetHeirsList());
            foreach (Node node in nodes)
            {
                if (node.Attachment == null || string.IsNullOrEmpty(node.Attachment.ToString()) ||
                    node.Card == null || node.IsDir == true || node.IsDel == true ||
                    string.IsNullOrEmpty(node.Title.Trim()) || string.IsNullOrEmpty(node.Card.Tag))
                {
                    continue;
                }
                NameArrayList.Add((node.Title.Trim(), node.Card.Tag));
                foreach (Card.Line.Tip tip in node.Card.Lines[0].Tips)
                {
                    NameArrayList.Add((tip.Content.Trim(), node.Card.Tag));
                }
            }
            string[] colorTags = { "搜索", "符号", "数字", "字母", "标记", "对话", "敏感", "建议",
                "角色", "龙套", "道具", "势力", "部门", "场景", "地区", "其他" };
            NameArrayList.Sort(new StringLengthComparer());
            foreach ((string, string) tuple in NameArrayList)
            {
                string el = Workflow.CheckStringMethod(tuple.Item2, colorTags);
                if (string.IsNullOrEmpty(el))
                {
                    HighlightingRule rule = NewRule(tuple.Item1, "未指定");
                    _syntax.MainRuleSet.Rules.Add(rule);
                }
                else
                {
                    HighlightingRule rule = NewRule(tuple.Item1, el);
                    _syntax.MainRuleSet.Rules.Add(rule);
                }
            }
            foreach (TextEditor textEditor in Gval.TextEditorList)
            {
                textEditor.SyntaxHighlighting = _syntax;
            }
        }

        /// <summary>
        /// 根据node对象从编辑器容器当中选定并返回当前的TabItem对象
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HandyControl.Controls.TabItem SelectItem(HandyControl.Controls.TabControl tabControl, Node node)
        {
            foreach (HandyControl.Controls.TabItem item in tabControl.Items)
            {
                if (item.Uid == node.Guid.ToString())
                {
                    item.IsSelected = true;
                    (item.Content as Editorkernel).ThisTextEditor.TextArea.Focus();
                    return item;
                }
            }
            return null;
        }


        /// <summary>
        /// 文字排版，并重新赋值给编辑框
        /// </summary>
        /// <param name="tEditor"></param>
        public static void TypeSetting(TextEditor tEditor)
        {
            if (tEditor == null)
            {
                return;
            }
            ICSharpCode.AvalonEdit.Document.DocumentLine oldLine = tEditor.Document.GetLineByOffset(tEditor.CaretOffset);
            int oldOffset = tEditor.CaretOffset;
            string reText = "　　"; //开头是两个全角空格
            string[] sArray = tEditor.Text.Split(new char[] { '\r', '\n', '\t' });
            string[] sArrayNoEmpty = sArray.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            foreach (string lineStr in sArrayNoEmpty)
            {
                //当前段落非空时，注意，这里的长度需要-1才是最后一个索引号
                if (Array.IndexOf(sArrayNoEmpty, lineStr) != sArrayNoEmpty.Length - 1)
                {
                    //非末尾的情况
                    reText += lineStr.Trim() + "\n　　\n　　";
                }
                else
                {
                    //末尾时不添加新行
                    reText += lineStr.Trim();
                }
            }
            //排版完成，重新赋值给文本框
            tEditor.Text = reText;
            if (Gval.EditorSettings.CursorToEnd == true)
            {
                //光标移动至文末
                tEditor.Select(tEditor.Text.Length, 0);
                tEditor.ScrollToEnd();
            }
            else
            {
                //光标移动至合适的位置
                tEditor.Select(Math.Min(tEditor.Text.Length, oldOffset), 0);
                tEditor.ScrollToLine(Math.Min(tEditor.LineCount, oldLine.LineNumber));
            }
            tEditor.Focus();
        }

        /// <summary>
        /// 退出时的检查
        /// </summary>
        /// <param name="LightEditor"></param>
        /// <param name="win"></param>
        public static void CloseLightEditor(Editorkernel LightEditor, Window win)
        {
            if (LightEditor.BtnSaveDoc.IsEnabled == true)
            {
                MessageBoxResult dr = MessageBox.Show("该章节尚未保存\n要在退出前保存更改吗？", "Tip", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
                if (dr == MessageBoxResult.Yes)
                {
                    LightEditor.BtnSaveText_Click(null, null);
                }
                if (dr == MessageBoxResult.No)
                {

                }
                if (dr == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            win.Close();
        }



    }
}
