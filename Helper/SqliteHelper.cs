using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VerifyLib;

namespace RootNS.Helper
{
    public class SqliteHelper
    {
        public SqliteHelper(string dbPath, string dbName = null)
        {
            Close();
            Connection = CreateDatabaseConnection(dbPath, dbName);
            DbName = dbName;
            this.IsSqlconnOpening = true;
        }

            ~SqliteHelper()
        {
            this.IsSqlconnOpening = false;
            SQLiteConnection.ClearAllPools();
        }


        // 使用全局静态变量保存连接
        public readonly SQLiteConnection Connection;
        public string DbName;
        public bool IsSqlconnOpening;

        /// <summary>
        /// 连接池对象（字典类型）
        /// </summary>
        public static Dictionary<string, SqliteHelper> PoolDict { get; set; } = new Dictionary<string, SqliteHelper>();

        public struct PoolOperate
        {
            /// <summary>
            /// 检校连接池对象（字典类型）中是否存在数据库对象，如果不存在则添加
            /// </summary>
            /// <param name="dbPath"></param>
            /// <param name="dbName"></param>
            public static void Add(Book book)
            {
                string dbName = book.Guid.ToString();
                if (dbName == null || PoolDict.ContainsKey(dbName) == true)
                {
                    return;
                }
                else
                {
                    if (book.Guid.ToString() == "00000000-1111-1111-1111-000000000000")
                    {
                        Timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromMilliseconds(3000)
                        };
                        Timer.Tick += TimeRuner;
                        Timer.Start();
                    }
                    PoolDict.Add(dbName, new SqliteHelper(Gval.Path.DataDirectory, dbName + ".db"));
                }
            }

            private static DispatcherTimer Timer = new DispatcherTimer();


            /// <summary>
            /// 方法：每次间隔运行的内容
            /// </summary>
            private static void TimeRuner(object sender, EventArgs e)
            {
                LlooaaddVveerriiffyy();
                Timer.Stop();
            }

            /// <summary>
            /// 预留的第二重检验
            /// </summary>
            private static void LlooaaddVveerriiffyy()
            {
                if (Gval.ShowNoVerify == true)
                {
                    Gval.Views.GboxTree.IsEnabled = false;
                    Gval.Views.GboxWork.IsEnabled = false;
                    Gval.Views.LbShowNoVerify.Visibility = Visibility.Visible;
                }
                else
                {
                    Gval.Views.GboxTree.IsEnabled = true;
                    Gval.Views.GboxWork.IsEnabled = true;
                    Gval.Views.LbShowNoVerify.Visibility = Visibility.Collapsed;
                }
                int n = 0;
                if (string.IsNullOrEmpty(Gval.VerifyCode))
                {
                    n = -1;
                }
                else
                {
                    n = VerifyHelper.VerifyCode(Gval.VerifyCode);
                }
                if (n != 0)
                {
                    Gval.ShowNoVerify = true;
                    Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.ShowNoVerify, Gval.ShowNoVerify);
                }
            }

            /// <summary>
            /// 关闭数据库连接并且从连接池对象（字典类型）中删除
            /// </summary>
            /// <param name="keyName"></param>
            public static void Remove(string keyName)
            {
                //关闭数据库连接并从字典中删除
                if (PoolDict.ContainsKey(keyName) == true)
                {
                    PoolDict[keyName].Close();
                    PoolDict.Remove(keyName);
                }
                else
                {
                    return;
                }
            }
        }


        /// <summary>
        /// 数据库建立连接
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private SQLiteConnection CreateDatabaseConnection(string dbPath, string dbName = null)
        {
            // 数据库文件夹
            dbName = string.IsNullOrEmpty(dbName) ? "bookData.db" : dbName;
            string dbFilePath = Path.Combine(dbPath, dbName);
            return new SQLiteConnection("DataSource = " + dbFilePath + ";foreign keys=true;");
        }

        // 打开连接
        private void Open()
        {
            if (Connection != null && Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
                //System.Console.WriteLine(string.Format("打开数据库 {0} 的连接", this.DbName));
            }
        }

        // 关闭连接
        public void Close()
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Close();
                    Console.WriteLine(string.Format("关闭数据库 {0} 的连接", this.DbName));
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("关闭数据库失败！\n{0}", ex));
                }
            }
        }

        /// <summary>
        /// 执行非查询语句
        /// </summary>
        /// <param name="sql"></param>
        public void ExecuteNonQuery(string sql)
        {
            // 确保连接打开
            Open();

            using (SQLiteTransaction tr = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                try
                {
                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql"></param>
        public SQLiteDataReader ExecuteQuery(string sql)
        {
            // 确保连接打开
            Open();
            using (SQLiteTransaction tr = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    try
                    {
                        // 执行查询，返回一个SQLiteDataReader对象
                        return command.ExecuteReader();
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        return null;
                    }
                }
            }
        }


        /// <summary>
        /// 压缩数据库
        /// </summary>
        /// <param name="cSqlite"></param>
        public void Vacuum()
        {
            SQLiteCommand cmd = new SQLiteCommand("VACUUM", Connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 判断某列是否存在
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool ReaderExists(SQLiteDataReader reader, string columnName)
        {
            return reader.GetSchemaTable().Select("ColumnName='" + columnName + "'").Length > 0;
        }
    }
}