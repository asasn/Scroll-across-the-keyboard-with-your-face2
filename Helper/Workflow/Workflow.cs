using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
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
            foreach (Node card in book.TabRoot.ChildNodes[5].GetHeirsList())
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
