using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Helper
{
    internal class TableHelper
    {
        /// <summary>
        /// 尝试建立一个书库数据库
        /// </summary>
        /// <param name="dbName"></param>
        public static void TryToBuildDatabaseForBook(Book book)
        {
            string sql = string.Empty;
            if (book.Guid.ToString() == "00000000-1111-1111-1111-000000000000")
            {
                sql += GetSqlStringForCreateBooksBankTable();
            }
            sql += GetSqlStringForCreateNodeTable();
            sql += GetSqlStringForCreateContentTable();
            sql += GetSqlStringForCreateSettingsTable();
            SqliteHelper.PoolOperate.Add(book);
            SqliteHelper.PoolDict[book.Guid.ToString()].ExecuteNonQuery(sql);
        }


        /// <summary>
        /// 为资料，全局设置建立基本的数据库："00000000-1111-1111-1111-000000000000.db"
        /// </summary>
        /// <returns></returns>
        private static string GetSqlStringForCreateBooksBankTable()
        {
            string sql = string.Empty;
            sql += string.Format("CREATE TABLE IF NOT EXISTS 书库 ([Index] INTEGER DEFAULT (0), Guid CHAR PRIMARY KEY, Title CHAR NOT NULL, Summary CHAR, Price DOUBLE DEFAULT (0), CurrentYear INTEGER DEFAULT (0), IsDel BOOLEAN DEFAULT(False));");
            sql += string.Format("CREATE INDEX IF NOT EXISTS 书库Guid ON 书库(Guid);");
            return sql;
        }

        /// <summary>
        /// 建立节点表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static string GetSqlStringForCreateNodeTable()
        {
            string sql = string.Empty;
            sql += string.Format("CREATE TABLE IF NOT EXISTS 节点 ([Index] INTEGER DEFAULT (0), Guid CHAR PRIMARY KEY, Puid CHAR, TypeName CHAR, IsDir BOOLEAN DEFAULT(False), IsExpanded BOOLEAN DEFAULT(False), IsChecked BOOLEAN DEFAULT(False), IsDel BOOLEAN DEFAULT(False));");
            sql += string.Format("CREATE INDEX IF NOT EXISTS 节点Guid ON 节点(Guid);");
            sql += string.Format("CREATE INDEX IF NOT EXISTS 节点Puid ON 节点(Puid);");
            return sql;
        }

        /// <summary>
        /// 建立节点内容表
        /// </summary>
        /// <returns></returns>
        private static string GetSqlStringForCreateContentTable()
        {
            string sql = string.Empty;
            sql += string.Format("CREATE TABLE IF NOT EXISTS 内容 (Guid CHAR  PRIMARY KEY REFERENCES 节点 (Guid) ON DELETE CASCADE ON UPDATE CASCADE, Title CHAR, Text TEXT, Summary CHAR, Count INTEGER (0), PointX DOUBLE DEFAULT (0), PointY DOUBLE DEFAULT (0));");
            sql += string.Format("CREATE INDEX IF NOT EXISTS 内容Guid ON 内容(Guid);");
            return sql;
        }
        private static string GetSqlStringForCreateSettingsTable()
        {
            string sql = string.Empty;
            sql += string.Format("CREATE TABLE IF NOT EXISTS 设置 (Key CHAR PRIMARY KEY, Value CHAR);");
            sql += string.Format("CREATE INDEX IF NOT EXISTS 设置Key ON 设置(Key);");
            return sql;
        }
    }
}
