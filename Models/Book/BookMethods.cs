using ICSharpCode.AvalonEdit.Highlighting;
using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Book : Entity
    {
        public Book()
        {
            TreeRoot.Owner = this;
            this.PropertyChanged += Book_PropertyChanged;
            this.TreeRoot.ChildNodes.CollectionChanged += ChildNodes_CollectionChanged;
            for (int i = 0; i < (int)TypeNameEnum.Count; i++)
            {
                var enumName = Enum.GetName(typeof(TypeNameEnum), i);
                string guid = String.Format("{0}-0000-0000-0000-000000000000", i.ToString().PadLeft(8, '0'));
                Node node = new Node() { TypeName = enumName, Guid = new Guid(guid) };
                TreeRoot.ChildNodes.Add(node);
            }
            this.CoverPath = Gval.Path.DataDirectory + this.Guid.ToString() + ".jpg";
            this.InitSyntax();
        }

        public void Insert()
        {

        }

        /// <summary>
        /// 初始化配色方案，填入本控件语法对象
        /// </summary>
        private void InitSyntax()
        {
            System.Xml.XmlTextReader xshdReader = new System.Xml.XmlTextReader(Gval.Path.XshdFilePath);
            this.Syntax = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xshdReader, HighlightingManager.Instance);
            xshdReader.Close();
        }



        /// <summary>
        /// 从TreeRoot开始，载入整本书的所有节点内容
        /// </summary>
        public void Load()
        {
            TableHelper.TryToBuildDatabaseForBook(this);
            SqliteHelper.PoolOperate.Add(this);
            Gval.FlagLoadingCompleted = false;
            foreach (var item in this.TreeRoot.ChildNodes)
            {
                item.ChildNodes.Clear();//载入之前先清空
                RecursiveReLoad(item);
            }
            Gval.FlagLoadingCompleted = true;
        }

        private void RecursiveReLoad(Node pNode)
        {
            string sql = string.Format("SELECT * FROM 节点 INNER JOIN 内容 ON 节点.Guid = 内容.Guid WHERE Puid='{0}' ORDER BY [Index];", pNode.Guid);
            SQLiteDataReader reader = SqliteHelper.PoolDict[this.Guid.ToString()].ExecuteQuery(sql);
            while (reader.Read())
            {
                Node node = new Node
                {
                    Guid = Guid.Parse(reader["Guid"].ToString()),
                    Index = Convert.ToInt32(reader["Index"]),
                    Title = reader["Title"] == DBNull.Value ? null : reader["Title"].ToString(),
                    Text = reader["Text"] == DBNull.Value ? null : reader["Text"].ToString(),
                    Summary = reader["Summary"] == DBNull.Value ? null : reader["Summary"].ToString(),
                    Attachment = reader["Attachment"] == DBNull.Value ? null : reader["Attachment"].ToString(),
                    Count = reader["Count"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Count"]),
                    PointX = Convert.ToDouble(reader["PointX"]),
                    PointY = Convert.ToDouble(reader["PointY"]),
                    IsDir = (bool)reader["IsDir"],
                    IsExpanded = (bool)reader["IsExpanded"],
                    IsChecked = (bool)reader["IsChecked"],
                    IsDel = (bool)reader["IsDel"],
                };
                pNode.ChildNodes.Add(node);
                RecursiveReLoad(node);
            }
            reader.Close();
        }

    }
}
