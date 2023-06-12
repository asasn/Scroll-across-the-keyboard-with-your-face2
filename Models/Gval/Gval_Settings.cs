using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNS.Models
{
    public partial class Gval : NotificationObject
    {
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
            public static string WebdavUrl { get { return "WebdavUrl"; } }
            public static string WebdavUserName { get { return "WebdavUserName"; } }
            public static string WebdavPassWord { get { return "WebdavPassWord"; } }
            public static string CursorToEnd { get { return "CursorToEnd"; } }
        }

        public struct EditorSettings
        {
            public static bool CursorToEnd { get; set; } = true;
        }

        /// <summary>
        /// Webdav相关
        /// </summary>

        public struct Webdav
        {
            public static string Url { get; set; }
            public static string UserName { get; set; }
            public static string PassWord { get; set; }
        }
    }
}
