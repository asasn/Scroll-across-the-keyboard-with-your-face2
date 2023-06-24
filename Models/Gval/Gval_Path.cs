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

        public struct Url
        {
            public static string HomePage { get { return "https://github.com/asasn/Scroll-across-the-keyboard-with-your-face2"; } }

            public static string Latest = "https://api.github.com/repos/asasn/Scroll-across-the-keyboard-with-your-face2/releases/latest";
        }
    }
}