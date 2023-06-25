using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace RootNS.Helper
{
    internal partial class Workflow
    {
        /// <summary>
        /// 程序开始时的数据载入流程，这里需要注意载入的先后顺序
        /// </summary>
        public static void Start()
        {
            if (FileIO.IsFolderExists(Gval.Path.DataDirectory) == false)
            {
                FileIO.TryToCreateFolder(Gval.Path.DataDirectory);
            }
            Gval.MaterialBook.Load();
            LoadSettings();
        }


        /// <summary>
        /// 再入基本的设置
        /// </summary>
        private static void LoadSettings()
        {
            LoadUserSign();
            LoadEditorSettings();
            LoadWebdavSettings();
        }

        private static void LoadUserSign()
        {
            object ret = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.HasBook);
            if (ret != null)
            {
                Gval.HasBook = Convert.ToBoolean(ret);
            }
            object ret2 = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.IsWarnAgain);
            if (ret2 != null)
            {
                Gval.IsWarnAgain = Convert.ToBoolean(ret2);
            }
        }


        private static void LoadEditorSettings()
        {
            object cursor = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.CursorToEnd);
            if (cursor != null)
            {
                Gval.EditorSettings.CursorToEnd = Convert.ToBoolean(cursor);
            }
        }


        private static void LoadWebdavSettings()
        {
            object url = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.WebdavUrl);
            object userName = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.WebdavUserName);
            object passWord = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.WebdavPassWord);
            if (url != null)
            {
                Gval.Webdav.Url = url.ToString();
            }
            if (userName != null)
            {
                Gval.Webdav.UserName = userName.ToString();
            }
            if (passWord != null)
            {
                Gval.Webdav.PassWord = passWord.ToString();
            }
        }

        /// <summary>
        /// 将资料库和所有书籍载入BooksBank对象当中
        /// 准备Gval.CurrentBook.Guid，SqliteHelper.PoolDict等必要的对象
        /// </summary>
        public static void LoadBooksToBank()
        {
            object curGuid = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.CurrentBookGuid);
            if (curGuid == null)
            {
                return;
            }
            else
            {
                Gval.CurrentBook.Guid = Guid.Parse(curGuid.ToString());
            }
            string sql = string.Format("SELECT * FROM 书库 ORDER BY [Index];");
            SQLiteDataReader reader = SqliteHelper.PoolDict[Gval.MaterialBook.Guid.ToString()].ExecuteQuery(sql);
            while (reader.Read())
            {
                Book book = new Book
                {
                    Guid = Guid.Parse(reader["Guid"].ToString()),
                    Index = Convert.ToInt32(reader["Index"]),
                    Title = reader["Title"].ToString(),
                    Summary = reader["Summary"].ToString(),
                    Price = Convert.ToDouble(reader["Price"]),
                    CurrentYear = Convert.ToInt64(reader["CurrentYear"]),
                    IsDel = (bool)reader["IsDel"],
                };
                if (Gval.CurrentBook.Guid == book.Guid)
                {
                    Gval.CurrentBook = book;
                    Gval.CurrentBook.Load();
                }
                Gval.BooksBank.Add(book);
            }
            reader.Close();
            Gval.FlagLoadingCompleted = true;
        }
    }
}
