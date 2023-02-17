using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Node : Entity
    {

        private double _pointX;
        /// <summary>
        /// 点位坐标X
        /// </summary>
        public double PointX
        {
            get { return _pointX; }
            set
            {
                _pointX = value;
                this.RaisePropertyChanged("PointX");
            }
        }

        private double _pointY;
        /// <summary>
        /// 点位坐标Y
        /// </summary>
        public double PointY
        {
            get { return _pointY; }
            set
            {
                _pointY = value;
                this.RaisePropertyChanged("PointY");
            }
        }

        private Node _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        public Node Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                this.RaisePropertyChanged("Parent");
            }
        }


        private bool _isDir;
        /// <summary>
        /// 是否目录
        /// </summary>
        public bool IsDir
        {
            get { return _isDir; }
            set
            {
                _isDir = value;
                this.RaisePropertyChanged("IsDir");
            }
        }


        private string _text = String.Empty;
        /// <summary>
        /// 文字内容
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        private ObservableCollection<object> _cGuidList = new ObservableCollection<object>();
        /// <summary>
        /// 子节点Guid集合
        /// </summary>
        public ObservableCollection<object> CGuidList
        {
            get { return _cGuidList; }
            set
            {
                _cGuidList = value;
                RaisePropertyChanged(nameof(CGuidList));
            }
        }

        private ObservableCollection<Node> _childNodes = new ObservableCollection<Node>();
        /// <summary>
        /// 子节点动态数据集合
        /// </summary>
        public ObservableCollection<Node> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                {
                    _childNodes = new ObservableCollection<Node>();
                }
                return _childNodes;
            }
            set
            {
                _childNodes = value;
                this.RaisePropertyChanged("ChildNodes");
            }
        }

        private int childsCount;
        /// <summary>
        /// 子节点数量统计
        /// </summary>
        public int ChildsCount
        {
            get { return childsCount; }
            set
            {
                childsCount = value;
                RaisePropertyChanged(nameof(ChildsCount));
            }
        }

        private int _count;
        /// <summary>
        /// 字数统计（一般是针对Text属性而言）
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        private bool _isExpanded;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                this.RaisePropertyChanged("IsExpanded");
            }
        }

        private bool _isChecked;
        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                this.RaisePropertyChanged("IsChecked");
            }
        }

        private bool _isSelected;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }



        private string _typeName = string.Empty;
        /// <summary>
        /// 节点类型（名称）
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                _typeName = value;
                this.RaisePropertyChanged(nameof(TypeName));
            }
        }

        private Book _owner;
        /// <summary>
        /// 隶属于哪一本书
        /// </summary>
        public Book Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                RaisePropertyChanged(nameof(Owner));
            }
        }


        //private Guid _ownerGuid;
        ///// <summary>
        ///// 隶属于哪一本书
        ///// </summary>
        //public Guid OwnerGuid
        //{
        //    get { return _ownerGuid; }
        //    set
        //    {
        //        _ownerGuid = value;
        //        this.RaisePropertyChanged(nameof(OwnerGuid));
        //    }
        //}




        private bool _reNameing;
        /// <summary>
        /// 是否正在命名状态
        /// </summary>
        public bool ReNameing
        {
            get { return _reNameing; }
            set
            {
                _reNameing = value;
                this.RaisePropertyChanged(nameof(ReNameing));
            }
        }

        /// <summary>
        /// 标题是否曾经改变的标志，配合Node_PropertyChanged事件当中的条件来使用
        /// </summary>
        private bool HasNameChange = false;

        /// <summary>
        /// （供搜索功能使用的临时变量）匹配的字符串数组
        /// </summary>
        public string[] Matches { get; set; }

        /// <summary>
        /// （供搜索功能使用的临时变量）临时标题
        /// </summary>
        public string TempTitle { get; set; }


        private object _tempToolTip;
        /// <summary>
        /// （供搜索功能使用的临时变量）悬浮显示内容
        /// </summary>
        public object TempToolTip
        {
            get { return _tempToolTip; }
            set
            {
                _tempToolTip = value;
                RaisePropertyChanged(nameof(TempToolTip));
            }
        }
    }
}
