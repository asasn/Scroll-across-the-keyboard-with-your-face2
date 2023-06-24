using RootNS.Models;
using System;
using System.Collections.Generic;
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
            if (Gval.IsNoBook)
            {
                Gval.IsNoBook = false;
                Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.IsNoBook, Gval.IsNoBook);
            }
            return newBook;
        }
    }
}
