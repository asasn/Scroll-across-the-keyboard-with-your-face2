using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    internal partial class Workflow
    {
        /// <summary>
        /// 创建一个书籍对象
        /// </summary>
        public static Book CreadANewBook()
        {
            Book newBook = new Book()
            {
                Index = Gval.BooksBank.Count,
                Title = "书籍" + (Gval.BooksBank.Count + 1),
            };
            Gval.BooksBank.Add(newBook);
            Gval.CurrentBook = newBook;
            TableHelper.TryToBuildDatabaseForBook(newBook);
            DataOut.InsertBooksBank(newBook);
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentBookGuid, newBook.Guid);
            if (Gval.HasBook == false)
            {
                Gval.HasBook = true;
                Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.HasBook, Gval.HasBook);
            }
            {
                Node node4 = new Node();
                node4.Title = "第一卷";
                node4.IsDir = true;
                newBook.TabRoot.ChildNodes[4].ChildNodes.Add(node4);
                node4.Insert();
            }
            string[] colorTags = { "角色", "龙套", "势力", "部门", "地区", "场景", "道具", "技能", "概念" };
            foreach (var colorTag in colorTags)
            {
                Node node = new Node();
                node.Title = colorTag;
                node.IsDir = true;
                newBook.TabRoot.ChildNodes[5].ChildNodes.Add(node);
                node.Insert();
            }

            return newBook;
        }
    }
}
