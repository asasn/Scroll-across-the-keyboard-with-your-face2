using RootNS.Helper;
using RootNS.MyControls;
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
        /// 控件和视图对象
        /// </summary>
        public struct Views
        {
            public static MainWindow MainWindow { get; set; }
            public static HandyControl.Controls.ComboBox BooksComboBox { get; set; }

            public static HandyControl.Controls.TabControl EditorTabControl { get; set; }

            public static UcShower UcShower { get; set; }
            public static UcSearch UcSearch { get; set; }
        }


    }
}
