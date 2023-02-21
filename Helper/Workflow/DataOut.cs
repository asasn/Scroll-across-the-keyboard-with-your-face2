using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    internal partial class DataOut
    {
        /// <summary>
        /// 在书库更新记录
        /// </summary>
        /// <param name="book"></param>
        public static void UpdateBookInfo(Book book)
        {
            string sql = string.Format("UPDATE 书库 SET [index]='{0}', Summary='{1}', Price='{2}', CurrentYear='{3}', IsDel='{4}' WHERE Guid='{5}';", book.Index, book.Summary.Replace("'", "''"), book.Price, book.CurrentYear, book.IsDel, book.Guid);
            SqliteHelper.PoolDict[Gval.MaterialBook.Guid.ToString()].ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 在书库中插入记录
        /// </summary>
        /// <param name="book"></param>
        public static Book InsertBooksBank(Book newBook)
        {
            string sql = string.Format("INSERT OR IGNORE INTO 书库 (Guid, [Index], Title, Summary) VALUES ('{0}', '{1}', '{2}', '{3}');", newBook.Guid, newBook.Index, newBook.Title.Replace("'", "''"), newBook.Summary.Replace("'", "''"));
            SqliteHelper.PoolDict[Gval.MaterialBook.Guid.ToString()].ExecuteNonQuery(sql);
            return newBook;
        }


    }
}
