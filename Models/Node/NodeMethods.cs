using RootNS.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
                if (this != this.Owner.TabRoot)//根节点之下的第一层tabRoot才是需要的目录树，所以排除总树根
                {
                    stuff.TypeName = this.TypeName;
                }
                //注意，这里要防止初始化加载的时候触发事件，需要加入Gval.FlagLoadingCompleted标记或者TabRoot.ChildNodes.Count的判断
                if (stuff.Owner.TabRoot.ChildNodes.Count > 5 &&
                    stuff.TypeName == stuff.Owner.TabRoot.ChildNodes[5].TypeName)
                {
                    stuff.GenerateNewCard();
                }
                if (Gval.FlagLoadingCompleted == true)
                {
                    UpdataBrothers(stuff);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                Node stuff = (Node)e.OldItems[0];
                this.ChildsCount -= 1;
                this.Count -= 1;
                if (Gval.FlagLoadingCompleted == true)
                {
                    UpdataBrothers(stuff);
                }
            }
        }

        /// <summary>
        /// 获取上一个操作的节点
        /// </summary>
        private Node GetPreviousNode()
        {
            Node previousNode;
            if (this.Owner.TabRoot.ChildNodes[0].ChildNodes.Count == 0 || this.Index == 0)
            {
                //草稿栏为空或者当前指向的项目索引为零时
                previousNode = this.Owner.TabRoot.ChildNodes[2].GetLastNode(false);
            }
            else
            {
                int i = this.Index;
                previousNode = this.Owner.TabRoot.ChildNodes[0].ChildNodes[i - 1];
            }
            return previousNode;
        }

        /// <summary>
        /// 获取上一个操作的节点在草稿栏目的索引
        /// </summary>
        private int GetPreviousIndex()
        {
            int pi = 0;
            if (this.Owner.TabRoot.ChildNodes[0].ChildNodes.Count == 0 || this.Index == 0)
            {

            }
            else
            {
                pi = this.Index;
            }
            return pi;
        }


        /// <summary>
        /// 从章节标题当中获取序号
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int GetNumberFromTitle()
        {
            int n = 0;
            Match match = Regex.Match(this.Title.Trim(), "第(.+?)章.*?");
            if (match.Success)
            {
                n = Convert.ToInt32(match.Value.Substring(1, match.Value.Length - 2));
            }
            return n;
        }

        /// <summary>
        /// 从章节标题当中获取名称（不包含序号的章节名）
        /// </summary>
        /// <returns></returns>
        private string GetNameFromTitle()
        {
            //标题的名称（排除序号）
            string cTitle = string.Empty;
            string[] rets;
            rets = Regex.Split(this.Title.Trim(), "第(.+?)章(.*?)");
            if (rets.Length == 4)
            {
                cTitle = rets[3].ToString().Trim();
            }
            return cTitle;
        }



        /// <summary>
        /// 更新下方的兄弟节点
        /// </summary>
        private void UpdataBrothers(Node stuff)
        {
            if (Gval.FlagLoadingCompleted == true)
            {
                if (stuff.Owner.TabRoot.ChildNodes.Count > 2 &&
                stuff.TypeName == stuff.Owner.TabRoot.ChildNodes[0].TypeName)
                {
                    Node previousNode = stuff.GetPreviousNode();
                    int pi = stuff.GetPreviousIndex();//前一个索引
                    //获取标题的基准序号
                    if (previousNode == null)
                    {
                        previousNode = new Node();
                    }
                    int fn = previousNode.GetNumberFromTitle();
                    //当前操作节点相对于finalDoc的偏移量
                    int off = 0;
                    for (int ii = pi; ii < stuff.Parent.ChildNodes.Count; ii++)
                    {
                        off++;
                        stuff.Parent.ChildNodes[ii].Title = string.Format("第{0}章 {1}", fn + off, stuff.Parent.ChildNodes[ii].GetNameFromTitle());
                    }
                    stuff.ChangeBrothersIndex(pi);
                    stuff.ChangeBrothersTitle(pi);
                }
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
            if (Gval.FlagLoadingCompleted == false) //重要：忽略载入过程当中的变化
            {
                return;
            }


            if (e.PropertyName == nameof(Title))
            {
                this.HasNameChange = true;
            }
            if (e.PropertyName == nameof(IsExpanded))
            {
                object propertyValue = this.GetType().GetProperty(e.PropertyName).GetValue(this, null);
                this.UpdateNodeProperty("节点", e.PropertyName, propertyValue.ToString());
            }
            if (e.PropertyName == nameof(IsChecked))
            {
                //节点的检查状态改变时，注意相关的算法
                if (this.IsChecked == true)
                {
                    //检查子节点
                    foreach (Node child in ChildNodes)
                    {
                        if (child.IsChecked == false)
                        {
                            child.IsChecked = true;
                        }
                    }
                    //检查父节点
                    if (this.Parent != null)
                    {
                        bool AllTrue = true;
                        foreach (Node brothers in Parent.ChildNodes)
                        {
                            if (brothers.IsChecked == false)
                            {
                                AllTrue = false;
                                break;
                            }
                        }
                        if (AllTrue && this.Parent.IsChecked == false)
                        {
                            this.Parent.IsChecked = true;
                        }
                    }
                }
                else
                {
                    //检查父节点
                    if (this.Parent != null)
                    {
                        if (this.Parent.IsChecked == true)
                        {
                            this.Parent.IsChecked = false;
                        }
                    }
                }
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
        /// 生成一张附属卡片（供信息卡管理页面使用）
        /// </summary>
        public void GenerateNewCard()
        {
            if (this.Attachment == null || string.IsNullOrEmpty(this.Attachment.ToString()))
            {
                this.Card = new Card();
                this.Attachment = JsonHelper.Otj<RootNS.Models.Card>(this.Card);
            }
            {
                this.Card = JsonHelper.Jto<RootNS.Models.Card>(this.Attachment.ToString());
            }
            this.ToolTip = new MyControls.CardShower(this);
        }

        /// <summary>
        /// 在目录树上的根节点（页面上显示的）
        /// </summary>
        public Node TreeRoot
        {
            get { return GetTreeRoot(); }
        }

        private Node GetTreeRoot()
        {
            Node root = this;
            while (this.Owner.TabRoot.ChildNodes.Contains(root.Parent) == false)
            {
                root = root.Parent;
            };
            return root;
        }

        /// <summary>
        /// 获取子节点当中的目录集合
        /// </summary>
        public ObservableCollection<Node> DirCollection
        {
            get { return GetDirCollection(); }
        }
        private ObservableCollection<Node> GetDirCollection()
        {
            ObservableCollection<Node> dirList = new ObservableCollection<Node>();
            foreach (Node node in this.ChildNodes)
            {
                if (node.IsDir == true)
                {
                    dirList.Add(node);
                }
            }
            return dirList;
        }



            /// <summary>
            /// 新建节点至表中（插入或忽略）
            /// </summary>
            /// <param name="book"></param>
            public void Insert()
        {
            string sql = string.Format("INSERT OR IGNORE INTO 节点 ([Index], Guid, Puid, TypeName, IsDir, IsExpanded, IsChecked, IsDel) VALUES ({0}, '{1}', '{2}', '{3}','{4}','{5}','{6}','{7}' );", this.Index, this.Guid, this.Parent.Guid, this.TypeName, this.IsDir, this.IsExpanded, this.IsChecked, this.IsDel);
            sql += string.Format("INSERT OR IGNORE INTO 内容 (Guid, Title, Text, Summary, Count, PointX, PointY, Attachment) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", this.Guid, this.Title.Replace("'", "''"), this.Text.Replace("'", "''"), this.Summary.Replace("'", "''"), this.Count, this.PointX, this.PointY, this.Attachment);
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

            ArrayList arrayList = this.GetHeirsList();
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
        public void ChangeBrothersIndex(int pn)
        {
            string sqlindex = String.Empty;
            for (int i = pn; i < this.Parent.ChildNodes.Count; i++)
            {
                if (i != this.Parent.ChildNodes[i].Index)
                {
                    //改变索引（被删除节点后面的节点统统前移一位）
                    this.Parent.ChildNodes[i].Index = i;
                    sqlindex += string.Format("UPDATE 节点 SET [Index]='{0}' WHERE Guid='{1}';", i, this.Parent.ChildNodes[i].Guid);
                }
            }
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sqlindex);
        }

        /// <summary>
        /// 移除或者插入之后，改变兄弟节点的章节名
        /// </summary>
        /// <param name="pn">改变处的索引位置</param>
        public void ChangeBrothersTitle(int pn)
        {
            string sqlindex = String.Empty;
            for (int i = pn; i < this.Parent.ChildNodes.Count; i++)
            {
                sqlindex += string.Format("UPDATE 内容 SET [Title]='{0}' WHERE Guid='{1}';", this.Parent.ChildNodes[i].Title, this.Parent.ChildNodes[i].Guid);
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
            ArrayList arrayList = this.GetHeirsList();
            string sqlDel = string.Empty;
            foreach (var item in arrayList)
            {
                (item as Node).IsDel = flag;
                sqlDel += string.Format("UPDATE 节点 SET IsDel='{0}' WHERE Guid='{1}';", this.IsDel, this.Guid);
            }
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sqlDel);

            this.Owner.UpdataSyntax();
        }

        /// <summary>
        /// 获取包括自身在内的后代列表（递归遍历子孙节点）
        /// </summary>
        /// <returns></returns>
        public ArrayList GetHeirsList(bool includeSelf = true)
        {
            ArrayList arrayList = new ArrayList();
            if (includeSelf == true)
            {
                arrayList.Add(this);
            }
            RecursiveTraversalChilds(this, arrayList);
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
                //提交之后，节点的标题改变，这个时候再来应用刷新高亮的方法
                if (this.TypeName == this.Owner.TabRoot.ChildNodes[5].TypeName)
                {
                    this.Owner.UpdataSyntax();
                }
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
            Gval.FlagLoadingCompleted = false;
            int pn = this.Parent.ChildNodes.IndexOf(this);//变动之前的索引位置
            this.Parent.ChildNodes.Remove(this);
            this.ChangeBrothersIndex(pn);//要在拥有新的父节点之前进行处理
            dstNode.ChildNodes.Add(this);
            this.UpdateNodeProperty("节点", "Puid", this.Parent.Guid.ToString());
            this.UpdateNodeProperty("节点", "Index", this.Index.ToString());
            this.UpdateNodeProperty("节点", "TypeName", this.TypeName);
            Gval.FlagLoadingCompleted = true;
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
        /// 获取节点全文
        /// </summary>
        /// <returns></returns>
        public string GetAllContent()
        {
            string strTitle = this.Title.ToString();
            string strText = this.Text.ToString();
            string strSummary = this.Summary.ToString();
            string age = this.PointX.ToString() + " " + this.PointY.ToString();
            string strCard = strTitle + " " + strText + " " + strSummary + " " + age + " ";
            if (this.Attachment != null)
            {
                strCard += this.Attachment.ToString();
            }
            string strContent = strTitle + " " + strText + " " + strSummary + " " + strCard;
            return strContent;
        }


        /// <summary>
        /// 更新一条节点记录
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        public void UpdateNodeProperty(string tableName, string fieldName, string value)
        {
            string sql = string.Format("UPDATE {0} SET [{1}]='{2}' WHERE Guid='{3}';", tableName, fieldName, value.Replace("'", "''"), this.Guid);
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sql);
        }


        /// <summary>
        /// 向下获取最后一个节点
        /// </summary>
        /// <param name="isDir">结果是否目录</param>
        /// <returns></returns>
        public Node GetLastNode(bool isDir = true)
        {
            ArrayList aList = this.GetHeirsList();
            //倒转，从最后开始遍历
            aList.Reverse();
            foreach (Node node in aList)
            {
                if (node.IsDir == isDir)
                {
                    return node;
                }
            }
            return null;
        }


        /// <summary>
        /// 从文本文档当中导入至章节
        /// </summary>
        public void Import()
        {
            Node parent;
            if (this.TypeName.Equals("草稿") ||
                this.TypeName.Equals("作品相关") ||
                this.TypeName.Equals("已发布")) { }
            else
            {
                return;
            }
            if (this.IsDir)
            {
                parent = this;
            }
            else
            {
                parent = this.Parent;
            }
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "文本文件(*.txt, *.book)|*.txt;*.book|所有文件(*.*)|*.*",
                Multiselect = true
            };

            string[] files;
            // 打开选择框选择
            if (dlg.ShowDialog() == true)
            {
                files = dlg.FileNames;
            }
            else
            {
                return;
            }
            string sqlImport = string.Empty;
            foreach (string srcFullFileName in files)
            {
                string[] rets = Regex.Split(FileIO.ReadFromTxt(srcFullFileName), "(\n第.+?章.*?\n)");

                string title = string.Empty;
                string content = string.Empty;
                foreach (string str in rets)
                {
                    Match match = Regex.Match(str, "(\n第.+?章.*?\n)");
                    if (match.Success)
                    {
                        title = match.Value;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(title))
                        {
                            title = System.IO.Path.GetFileNameWithoutExtension(srcFullFileName);
                        }
                        content = str;
                    }
                    if (string.IsNullOrEmpty(content))
                    {
                        continue;
                    }
                    Node newNode = new Node
                    {
                        Title = title.Trim(),
                        Text = "　　" + content.Trim(),
                    };
                    newNode.Count = CommonHelper.Count.QiDianCount(newNode.Text);
                    parent.ChildNodes.Add(newNode);
                    sqlImport += string.Format("INSERT OR IGNORE INTO 节点 ([Index], Guid, Puid, TypeName, IsDir, IsExpanded, IsChecked, IsDel) VALUES ({0}, '{1}', '{2}', '{3}','{4}','{5}','{6}','{7}' );", newNode.Index, newNode.Guid, newNode.Parent.Guid, newNode.TypeName, newNode.IsDir, newNode.IsExpanded, newNode.IsChecked, newNode.IsDel);
                    sqlImport += string.Format("INSERT OR IGNORE INTO 内容 (Guid, Title, Text, Summary, Count, PointX, PointY, Attachment) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", newNode.Guid, newNode.Title.Replace("'", "''"), newNode.Text.Replace("'", "''"), newNode.Summary.Replace("'", "''"), newNode.Count, newNode.PointX, newNode.PointY, newNode.Attachment);
                    title = string.Empty;
                    content = string.Empty;
                }

            }
            SqliteHelper.PoolDict[this.Owner.Guid.ToString()].ExecuteNonQuery(sqlImport);
        }

        /// <summary>
        /// 导出至文本文档
        /// </summary>
        public void Export()
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            folder.Description = "选择文件所在文件夹目录";  //提示的文字
            folder.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folder.SelectedPath;
                string fileName = this.Owner.Title + "_" + this.Title;
                if (FileIO.IsFolderExists(path) == false)
                {
                    FileIO.TryToCreateFolder(path);
                }
                string content = string.Empty;
                foreach (Node node in this.GetHeirsList())
                {
                    content += node.Title + "\n";
                    content += node.Text + "\n\n";
                }
                ExportMethod(fileName, path, content);
            }
        }

        private void ExportMethod(string fileName, string path, string content)
        {
            string fullFileName = String.Format("{0}/{1}.txt", path, fileName);
            int n = 1;
            while (FileIO.IsFileExists(fullFileName) == true)
            {
                fullFileName = String.Format("{0}/{1} - {2}.txt", path, fileName, n);
                n++;
            }
            FileIO.WriteToTxt(fullFileName, content);
        }
    }
}
