using RootNS.Helper;
using System;
using System.Collections.Generic;
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
        /// 常用路径
        /// </summary>
        public struct Path
        {
            public static string AppBaseDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
            public static string DataDirectory { get { return AppDomain.CurrentDomain.BaseDirectory + "/Data/"; } }
            public static string ResourcesDirectory { get { return AppDomain.CurrentDomain.BaseDirectory + "/Resources/"; } }
            public static string XshdFilePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Text.xshd";

        }


        /// <summary>
        /// 设置的键名
        /// </summary>
        public struct SettingsKeys
        {
            public static string CurrentBookGuid { get { return "CurrentBookGuid"; } }
            public static string CurrentThemeColor { get { return "CurrentThemeColor"; } }
            public static string TomatoTimeSetTotalMinutes { get { return "TomatoTimeSetTotalMinutes"; } }
            public static string Scroll2End { get { return "Scroll2End"; } }
            public static string FontSizeBypt { get { return "FontSizeBypt"; } }
            
        }
    }
}
