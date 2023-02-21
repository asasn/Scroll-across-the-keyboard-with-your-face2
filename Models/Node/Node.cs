using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RootNS.Models
{
    public partial class Node : Entity
    {

        private Card _card = new Card();
        /// <summary>
        /// 卡片对象
        /// </summary>
        public Card Card
        {
            get { return _card; }
            set
            {
                _card = value;
                RaisePropertyChanged(nameof(Card));
            }
        }

        private object _toolTip;
        /// <summary>
        /// 悬浮显示内容
        /// </summary>
        public object ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                RaisePropertyChanged(nameof(ToolTip));
            }
        }

        private bool _hasChange;
        /// <summary>
        /// 运行时变量：供信息卡使用
        /// </summary>
        public bool HasChange
        {
            get { return _hasChange; }
            set
            {
                _hasChange = value;
                RaisePropertyChanged(nameof(HasChange));
            }
        }

        private Visibility _visibility;
        /// <summary>
        /// 是否显示
        /// </summary>
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                RaisePropertyChanged(nameof(Visibility));
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



    }
}
