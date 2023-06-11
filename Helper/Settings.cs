using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    internal class Settings
    {
        /// <summary>
        /// 获取设置值
        /// </summary>
        /// <param name="dbName">当前工作的书籍名称/数据库名称</param>
        /// <param name="key">设置键</param>
        /// <returns></returns>
        public static object Get(Book book, string key)
        {
            if (book.Guid == null || SqliteHelper.PoolDict.ContainsKey(book.Guid.ToString()) == false)
            {
                return null;
            }
            object value = null;
            string sql = string.Format("SELECT * FROM 设置 where Key='{0}';", key.Replace("'", "''"));
            SQLiteDataReader reader = SqliteHelper.PoolDict[book.Guid.ToString()].ExecuteQuery(sql);
            while (reader.Read())
            {
                value = reader["Value"];
            }
            reader.Close();
            return value;
        }

        /// <summary>
        /// 保存设置值
        /// </summary>
        /// <param name="dbName">当前工作的书籍名称/数据库名称</param>
        /// <param name="key">设置键</param>
        /// <param name="value">设置值</param>
        public static void Set(Book book, string key, object value)
        {
            if (book.Guid == null || value == null || SqliteHelper.PoolDict.ContainsKey(book.Guid.ToString()) == false)
            {
                return;
            }
            string sql = string.Format("REPLACE INTO 设置 (Key, Value) VALUES ('{0}', '{1}');", key.Replace("'", "''"), value.ToString().Replace("'", "''"));
            SqliteHelper.PoolDict[book.Guid.ToString()].ExecuteNonQuery(sql);
        }

    }
}
