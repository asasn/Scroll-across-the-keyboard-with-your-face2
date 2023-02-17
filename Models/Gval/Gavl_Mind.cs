using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public partial class Gval : NotificationObject
    {
        /// <summary>
        /// 静态事件处理属性更改
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        /// <summary>
        /// 初始化
        /// </summary>
        public Gval()
        {
            StaticPropertyChanged += Gval_StaticPropertyChanged;
        }

        private void Gval_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}
