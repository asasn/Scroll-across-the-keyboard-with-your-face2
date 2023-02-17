using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RootNS.Models
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public partial class Gval : NotificationObject
    {
        /// <summary>
        /// 默认主题颜色
        /// </summary>
        public static SolidColorBrush DefaultThemeColor { get; } = Brushes.DodgerBlue;

        private static SolidColorBrush _currentThemeColor = Brushes.DodgerBlue;
        /// <summary>
        /// 当前主题颜色
        /// </summary>
        public static SolidColorBrush CurrentThemeColor
        {
            get { return _currentThemeColor; }
            set
            {
                _currentThemeColor = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentThemeColor)));
            }
        }



    }
}
