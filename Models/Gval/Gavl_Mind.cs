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
        /// 初始化
        /// </summary>
        public Gval()
        {
            StaticPropertyChanged += Gval_StaticPropertyChanged;
        }

        /// <summary>
        /// 静态事件处理属性更改
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        private void Gval_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private static string _currentVersion = "2.0.3.0";

        public static string CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                _currentVersion = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentVersion)));
            }
        }

        private static string _appAuthor = "不问苍生问鬼神";

        public static string AppAuthor
        {
            get { return _appAuthor; }
            set
            {
                _appAuthor = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppAuthor)));
            }
        }
    }
}
