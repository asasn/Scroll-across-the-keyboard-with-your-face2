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
            //光标移动至文末 
            //tEditor.ScrollToEnd();
            //tEditor.Select(tEditor.Text.Length, 0);
            //光标移动至合适的位置
            tEditor.ScrollToLine(Math.Min(tEditor.LineCount, oldLine.LineNumber));
            tEditor.Select(Math.Min(tEditor.Text.Length, oldOffset), 0);
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
