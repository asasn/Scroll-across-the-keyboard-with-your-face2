using RootNS.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Node : Entity
    {
        public Node()
        {
            ChildNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnMoreStuffChanged);
            this.PropertyChanged += Node_PropertyChanged;
        }

        /// <summary>
        /// 数据集合变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnMoreStuffChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnCGuidListChange();
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Node stuff = (Node)e.NewItems[0];
                this.IsDir = true;
                stuff.Owner = this.Owner;
                stuff.Parent = this;
                stuff.Index = this.ChildNodes.IndexOf(stuff);
                this.ChildsCount += 1;
                this.Count += 1;
                if (this != this.Owner.TreeRoot)//根节点之下的第一层tabRoot才是需要的目录树，所以排除总树根
                {
                    stuff.TypeName = this.TypeName;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                Node stuff = (Node)e.OldItems[0];
                this.ChildsCount -= 1;
                this.Count -= 1;
            }
        }
        /// <summary>
        /// 节点属性变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Gval.FlagLoadingCompleted == false) //忽略载入过程当中的变化
            {
                return;
            }
            if (e.PropertyName == nameof(Title))
            {
                this.HasNameChange = true;
            }
            if (
                e.PropertyName == nameof(IsChecked) ||
                e.PropertyName == nameof(IsExpanded))
            {
                object propertyValue = this.GetType().GetProperty(e.PropertyName).GetValue(this, null);
                this.UpdateNodeProperty("节点", e.PropertyName, propertyValue.ToString());
            }
        }

        /// <summary>
        /// 专供子节点集合变动时调用的函数
        /// </summary>
        private void OnCGuidListChange()
        {
            this.CGuidList.Clear();
            foreach (Node node in this.ChildNodes)
            {
                CGuidList.Add(node.Guid);
            }
        }

        public void Load()
        {

        }

        /// <summary>
        /// 把节点保存进书库中
        /// </summary>
        /// <param name="book"></param>
        public void Save()
        {
            string sql = string.Format("INSERT OR IGNORE INTO 节点 ([Index], Guid, Puid, TypeName, IsDir, IsExpanded, IsChecked, IsDel) VALUES ({0}, '{1}', '{2}', '{3}','{4}','{5}','{6}','{7}' );", this.Index, this.Guid, this.Parent.Guid, this.TypeName, this.IsDir, this.IsExpanded, this.IsChecked, this.IsDel);
            sql += string.Format("INSERT OR IGNORE INTO 内容 (Guid, Title, Text, Summary, Count, PointX, PointY) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", this.Guid, this.Title.Replace("'", "''"), this.Text.Replace("'", "''"), this.Summary.Replace("'", "''"), this.Count, this.PointX, this.PointY);
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sql);
        }


        public void Remove()
        {
            if (Gval.OpeningDocList.Contains(this))
            {
                Gval.OpeningDocList.Remove(this);
            }
            if (this.IsDel == true)
            {
                this.RealRemove();
                this.ReSelect();
            }
            else
            {
                this.ChangeDelFlag(true);
            }
        }

        /// <summary>
        /// 真实删除
        /// </summary>
        private void RealRemove()
        {
            int pn = this.Parent.ChildNodes.IndexOf(this);//变动之前的索引位置
            this.Parent.ChildNodes.Remove(this);

            ArrayList arrayList = this.GetChildNodesList();
            string sqlDel = string.Empty;
            foreach (var item in arrayList)
            {
                sqlDel += string.Format("DELETE FROM 节点 WHERE Guid='{0}';", this.Guid);
            }
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sqlDel);

            this.ChangeBrothersIndex(pn);
        }

        /// <summary>
        /// 移除或者插入之后，改变兄弟节点的索引
        /// </summary>
        /// <param name="pn">改变处的索引位置</param>
        private void ChangeBrothersIndex(int pn)
        {
            string sqlindex = String.Empty;
            for (int i = pn; i < this.Parent.ChildNodes.Count; i++)
            {
                //改变索引（被删除节点后面的节点统统前移一位）
                this.Parent.ChildNodes[i].Index = i;
                sqlindex += string.Format("UPDATE 节点 SET [Index]='{0}' WHERE Guid='{1}';", i, this.Parent.ChildNodes[i].Guid);
            }
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sqlindex);
        }


        /// <summary>
        /// 删除当前节点值后重新选择新节点
        /// </summary>
        private void ReSelect()
        {
            int i = 0;
            if (this.Parent.ChildNodes.Count > 0)
            {
                if (this.Index >= 0)
                {
                    i = this.Index;
                }
                if (this.Index == this.Parent.ChildNodes.Count)
                {
                    i = this.Index - 1;
                }
                this.Parent.ChildNodes[i].IsSelected = true;
            }
        }

        /// <summary>
        /// 撤销删除
        /// </summary>
        public void UnDel()
        {
            this.ChangeDelFlag(false);
        }

        /// <summary>
        /// 改变删除标志
        /// </summary>
        /// <param name="flag"></param>
        private void ChangeDelFlag(bool flag)
        {
            this.IsDel = flag;
            ArrayList arrayList = this.GetChildNodesList();
            string sqlDel = string.Empty;
            foreach (var item in arrayList)
            {
                (item as Node).IsDel = flag;
                sqlDel += string.Format("UPDATE 节点 SET IsDel='{0}' WHERE Guid='{1}';", this.IsDel, this.Guid);
            }
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sqlDel);
        }

        /// <summary>
        /// 获取包括自身在内的子节点列表
        /// </summary>
        /// <returns></returns>
        private ArrayList GetChildNodesList()
        {
            ArrayList arrayList = new ArrayList();
            RecursiveTraversalChilds(this, arrayList);
            arrayList.Add(this);
            return arrayList;
        }

        /// <summary>
        /// 递归遍历子节点
        /// </summary>
        /// <param name="curNode"></param>
        /// <param name="arrayList"></param>
        private void RecursiveTraversalChilds(Node curNode, ArrayList arrayList)
        {
            for (int i = 0; i < curNode.ChildNodes.Count; i++)
            {
                Node stuff = curNode.ChildNodes[i];
                RecursiveTraversalChilds(stuff, arrayList);
                arrayList.Add(stuff);
            }
        }

        /// <summary>
        /// 完成节点标题的重命名
        /// </summary>
        public void FinishRename()
        {
            this.ReNameing = false;
            if (HasNameChange)
            {
                CommitReName();
                HasNameChange = false;
            }
        }

        /// <summary>
        /// 提交节点标题的重命名结果
        /// </summary>
        private void CommitReName()
        {
            this.UpdateNodeProperty("内容", "Title", this.Title);
        }


        /// <summary>
        /// 添加至指定节点的子节点末尾
        /// </summary>
        /// <param name="dstNode">指定根节点</param>
        public void MoveTo(Node dstNode)
        {
            if (dstNode.IsDir == false)
            {
                return;
            }
            int pn = this.Parent.ChildNodes.IndexOf(this);//变动之前的索引位置
            this.Parent.ChildNodes.Remove(this);
            this.ChangeBrothersIndex(pn);//要在拥有新的父节点之前进行处理
            dstNode.ChildNodes.Add(this);
            this.UpdateNodeProperty("节点", "Puid", this.Parent.Guid.ToString());
            this.UpdateNodeProperty("节点", "Index", this.Index.ToString());
            this.UpdateNodeProperty("节点", "TypeName", this.TypeName);
        }

        /// <summary>
        /// 节点向上移动一位
        /// </summary>
        public void MoveUp()
        {
            if (this.Index <= 0)
            {
                return;
            }
            UpDown(this.Index, this.Index - 1);
        }

        /// <summary>
        /// 节点向下移动一位
        /// </summary>
        public void MoveDown()
        {
            if (this.Index >= this.Parent.ChildNodes.Count - 1)
            {
                return;
            }
            UpDown(this.Index, this.Index + 1);
        }


        private void UpDown(int a, int b)
        {
            this.Parent.ChildNodes.Move(a, b);
            this.Parent.ChildNodes[a].Index = a;
            this.Parent.ChildNodes[b].Index = b;
            this.Parent.ChildNodes[a].UpdateNodeProperty("节点", "Index", a.ToString());
            this.Parent.ChildNodes[b].UpdateNodeProperty("节点", "Index", b.ToString());
        }

        

        /// <summary>
        /// 更新一条节点记录
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        private void UpdateNodeProperty(string tableName, string fieldName, string value)
        {
            string sql = string.Format("UPDATE {0} SET [{1}]='{2}' WHERE Guid='{3}';", tableName, fieldName, value.Replace("'", "''"), this.Guid);
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sql);
        }
    }
}
