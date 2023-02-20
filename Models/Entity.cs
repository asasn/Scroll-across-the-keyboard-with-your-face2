using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    /// <summary>
    /// 本书的对象实体，同时为Book，Node对象的基类
    /// </summary>
    public class Entity : NotificationObject
    {

        private int _index;
        /// <summary>
        /// 索引序号
        /// </summary>
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                this.RaisePropertyChanged("Index");
            }
        }

        private Guid _guid = Guid.NewGuid();
        /// <summary>
        /// 全局唯一标识符
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set
            {
                _guid = value;
                RaisePropertyChanged(nameof(Guid));
            }
        }

        private string _title = "新节点";
        /// <summary>
        /// 标题（名称）
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }



        private string _summary = String.Empty;
        /// <summary>
        /// 简介/备忘
        /// </summary>
        public string Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                this.RaisePropertyChanged("Summary");
            }
        }


        private bool _isDel;
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool IsDel
        {
            get { return _isDel; }
            set
            {
                _isDel = value;
                this.RaisePropertyChanged(nameof(IsDel));
            }
        }



        private object _attachment = String.Empty;
        /// <summary>
        /// 附件
        /// </summary>
        public object Attachment
        {
            get { return _attachment; }
            set
            {
                _attachment = value;
                RaisePropertyChanged(nameof(Attachment));
            }
        }
    }
}
