using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace RootNS.Helper
{
    /// <summary>
    /// 工作流（内部业务）
    /// </summary>
    internal partial class Workflow
    {

        /// <summary>
        /// 从章节标题当中获取序号
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int GetNumberFromTitle(string title)
        {
            int n = 0;
            Match match = Regex.Match(title.Trim(), "第(.+?)章.*?");
            if (match.Success)
            {
                n = Convert.ToInt32(match.Value.Substring(1, match.Value.Length - 2));
            }
            return n;
        }

        /// <summary>
        /// 从章节标题当中获取"第xx章样式"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetTitleFromTitle(string title)
        {
            string cTitle = string.Empty;
            Match match = Regex.Match(title.Trim(), "第(.+?)章.*?");
            if (match.Success)
            {
                cTitle = match.Value;
            }
            return cTitle;
        }

        /// <summary>
        /// 从章节标题当中获取名称（不包含序号的章节名）
        /// </summary>
        /// <returns></returns>
        public static string GetNameFromTitle(string title)
        {
            //标题的名称（排除序号）
            string cTitle = string.Empty;
            string[] rets;
            rets = Regex.Split(title.Trim(), "第(.+?)章(.*?)");
            if (rets.Length == 4)
            {
                cTitle = rets[3].ToString().Trim();
            }
            return cTitle;
        }

        /// <summary>
        /// 检查字符串中是否包含某个集合当中的元素，并且把第一个匹配的结果拾取出来：foreach循环(非LINQ表达式)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="excludeWordList"></param>
        /// <returns></returns>
        public static string CheckStringMethod(string str, ICollection<string> excludeWordList)
        {
            string contain = string.Empty;
            if (str.Trim().Length <= 0 || excludeWordList == null || excludeWordList.Count <= 0)
            {
                return contain;
            }
            foreach (var el in excludeWordList)
            {
                if (str.Contains(el))
                {
                    contain = el;
                    break; ;
                }
            }
            return contain;
        }

        /// <summary>
        /// 根据名称查找命令
        /// </summary>
        /// <param name="commandBindings"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public static ICommand FindByName(CommandBindingCollection commandBindings, string commandName)
        {
            foreach (CommandBinding cb in commandBindings)
            {
                RoutedCommand rc = cb.Command as RoutedCommand;
                if (commandName == rc.Name)
                {
                    return cb.Command;
                }
            }
            return null;
        }


        /// <summary>
        /// 获取匹配的信息卡片
        /// </summary>
        /// <param name="match"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Node GetMatchCard(System.Text.RegularExpressions.Match match, Book book)
        {
            System.Collections.ArrayList cards = new System.Collections.ArrayList();
            cards.AddRange(Gval.CurrentBook.TabRoot.ChildNodes[5].GetHeirsList());
            cards.AddRange(Gval.MaterialBook.TabRoot.ChildNodes[8].GetHeirsList());
            foreach (Node card in cards)
            {
                if (card.Attachment == null || card.Card == null || card.IsDir == true)
                {
                    continue;
                }
                if (match.Value.Equals(card.Title.Trim()))
                {
                    return card;
                }
                foreach (Card.Line.Tip tip in card.Card.Lines[0].Tips)
                {
                    if (match.Value.Equals(tip.Content.Trim()))
                    {
                        return card;
                    }
                }
            }
            return null;
        }


        //-----------------------------------------------以下为私有方法-----------------------------------------------





    }
}
