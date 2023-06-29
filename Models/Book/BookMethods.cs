using ICSharpCode.AvalonEdit.Highlighting;
using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Book : Entity
    {
        public Book()
        {
            TabRoot.Owner = this;
            this.PropertyChanged += Book_PropertyChanged;
            this.TabRoot.ChildNodes.CollectionChanged += ChildNodes_CollectionChanged;
            for (int i = 0; i < (int)TypeNameEnum.Count; i++)
            {
                var enumName = Enum.GetName(typeof(TypeNameEnum), i);
                string guid = String.Format("{0}-0000-0000-0000-000000000000", i.ToString().PadLeft(8, '0'));
                Node node = new Node() { IsDir = true, TypeName = enumName, Guid = new Guid(guid) };
                TabRoot.ChildNodes.Add(node);
            }
            TabRoot.ChildNodes[5].Title = "未指定";
            TabRoot.ChildNodes[8].Title = "未指定";
            this.CoverPath = Gval.Path.DataDirectory + this.Guid.ToString() + ".jpg";
            if (FileIO.IsFileExists(this.CoverPath) == false)
            {
                this.CoverPath = "../Resources/DefaultCover.jpg";
            }
        }






        private void ChildNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //this.CGuidList = TabRoot.CGuidList;
        }

        private void Book_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        public void Insert()
        {

        }


        double counter;
        double total = 1;
        /// <summary>
        /// 从TabRoot开始，递归载入整本书的所有节点内容
        /// </summary>
        public void Load()
        {
            if (this.Loaded == true)
            {
                return;
            }
            TableHelper.TryToBuildDatabaseForBook(this);
            SqliteHelper.PoolOperate.Add(this);
            counter = 0;
            object _total = Settings.Get(this, Gval.SettingsKeys.ThisBookTotalNodesCount);
            if (_total != null)
            {
                total = Convert.ToDouble(_total);
            }
            Gval.PercentPoint = 0;
            foreach (var item in this.TabRoot.ChildNodes)
            {
                item.ChildNodes.Clear();//载入之前先清空
                RecursiveReLoad(item);
            }
            Settings.Set(this, Gval.SettingsKeys.ThisBookTotalNodesCount, counter);
            this.Loaded = true;
        }

        private void RecursiveReLoad(Node pNode)
        {
            string sql = string.Format("SELECT * FROM 节点 INNER JOIN 内容 ON 节点.Guid = 内容.Guid WHERE Puid='{0}' ORDER BY [Index];", pNode.Guid);
            SQLiteDataReader reader = SqliteHelper.PoolDict[this.Guid.ToString()].ExecuteQuery(sql);
            while (reader.Read())
            {
                Node node = new Node
                {
                    //在创建的时候就预设Owner，防止后面Attachment属性变动时出错
                    Owner = this,
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
                    HasSave = true
                };
                pNode.ChildNodes.Add(node);
                counter++;
                Gval.PercentPoint = Math.Round(counter / total, 2);
                if (node.TypeName == node.Owner.TabRoot.ChildNodes[5].TypeName ||
                    node.TypeName == node.Owner.TabRoot.ChildNodes[8].TypeName)
                {
                    node.GenerateNewCard();
                }
                RecursiveReLoad(node);
            }
            reader.Close();
        }

        /// <summary>
        /// 关闭本书
        /// </summary>
        public void Close()
        {
            if (Gval.Views.EditorTabControl == null)
            {
                return;
            }
            //当移除完元素之后，数组大小发生了变更，会抛出异常，所以在这里使用逆序遍历来进行删除
            for (int i = Gval.Views.EditorTabControl.Items.Count - 1; i >= 0; i--)
            {
                Gval.Views.EditorTabControl.Items.Remove(Gval.Views.EditorTabControl.Items[i]);
            }
        }

    }
}
