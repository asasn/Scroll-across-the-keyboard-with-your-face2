using ICSharpCode.AvalonEdit.Highlighting;
using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Book : Entity
    {

        public enum TypeNameEnum
        {
            草稿,
            作品相关,
            已发布,
            大事记,
            故事大纲,
            文章片段,
            情节设计,
            信息卡,
            全局资料管理,
            全局题材管理,
            全局文章片段,
            全局情节设计,
            全局信息卡,
            全局灵感管理,
            Count,
        }



        //private Guid _tabRootGuid;
        ///// <summary>
        ///// 根节点Guidid
        ///// </summary>
        //public Guid TabRootGuid
        //{
        //    get { return _tabRootGuid; }
        //    set
        //    {
        //        _tabRootGuid = value;
        //        RaisePropertyChanged(nameof(TabRootGuid));
        //    }
        //}


        private Node _tabRoot = new Node() { Guid = new Guid(), TypeName="根节点"};
        /// <summary>
        /// 书籍目录树的最顶部根节点
        /// </summary>
        public Node TabRoot
        {
            get { return _tabRoot; }
            set
            {
                _tabRoot = value;
                RaisePropertyChanged(nameof(TabRoot));
            }
        }

        private long _currentYear;
        /// <summary>
        /// 书中剧情年份，当前年份
        /// </summary>
        public long CurrentYear
        {
            get { return _currentYear; }
            set
            {
                _currentYear = value;
                RaisePropertyChanged(nameof(CurrentYear));
            }
        }

        private long _bornYear;
        /// <summary>
        /// 书中参考年份，一般是主角的出生年份
        /// </summary>
        public long BornYear
        {
            get { return _bornYear; }
            set
            {
                _bornYear = value;
                RaisePropertyChanged(nameof(BornYear));
            }
        }


        private double _pirce;
        /// <summary>
        /// 本书稿酬单价
        /// </summary>
        public double Price
        {
            get { return _pirce; }
            set
            {
                _pirce = value;
                this.RaisePropertyChanged("Price");
            }
        }


        private string _coverpath;
        /// <summary>
        /// 封面路径
        /// </summary>
        public string CoverPath
        {
            get { return _coverpath; }
            set
            {
                _coverpath = value;
                this.RaisePropertyChanged("CoverPath");
            }
        }


        private IHighlightingDefinition _syntax;
        /// <summary>
        /// 语法高亮对象
        /// </summary>
        public IHighlightingDefinition Syntax
        {
            get { return _syntax; }
            set
            {
                _syntax = value;
                RaisePropertyChanged(nameof(Syntax));
            }
        }

        private Node _selectedNode;
        /// <summary>
        /// 选中的节点
        /// </summary>
        public Node SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                this.RaisePropertyChanged("SelectedNode");
            }
        }


        //private ObservableCollection<object> _cGuidList = new ObservableCollection<object>();
        ///// <summary>
        ///// 子节点Guid集合
        ///// </summary>
        //public ObservableCollection<object> CGuidList
        //{
        //    get { return _cGuidList; }
        //    set
        //    {
        //        _cGuidList = value;
        //        RaisePropertyChanged(nameof(CGuidList));
        //    }
        //}
    }
}
