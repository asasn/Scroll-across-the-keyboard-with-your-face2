﻿using ICSharpCode.AvalonEdit.Highlighting;
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
            事件记录,
            文章片段,
            信息卡,
            全局情节设计,
            全局文章片段,
            全局信息卡,
            全局资料管理,
            全局题材管理,
            全局灵感管理,
            云草稿,
            云大纲,
            Count,
        }

        private bool _loaded;
        /// <summary>
        /// 检查本书是否已经载入，如果已经载入，就不用再重新从数据库读取了，切换当前书籍时进行检查使用
        /// </summary>
        public bool Loaded
        {
            get { return _loaded; }
            set
            {
                _loaded = value;
                RaisePropertyChanged(nameof(Loaded));
            }
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


        private Node _tabRoot = new Node() { Guid = new Guid(), TypeName = "根节点" };
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

        private ObservableCollection<Outline> _outLines;
        /// <summary>
        /// 大纲
        /// </summary>
        public ObservableCollection<Outline> OutLines
        {
            get { return _outLines; }
            set
            {
                _outLines = value;
                RaisePropertyChanged(nameof(OutLines));
            }
        }

        public struct Outline
        {
            public string Title;
            public string Content;
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
