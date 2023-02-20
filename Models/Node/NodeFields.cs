using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Node : Entity
    {
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

        private double _pointX;
        /// <summary>
        /// 储存在表中的字段：点位坐标X
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
        /// 储存在表中的字段：点位坐标Y
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


    }
}
