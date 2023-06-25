using RootNS.Helper;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RootNS.Models
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public partial class Gval : NotificationObject
    {

        public static HandyControl.Controls.ComboBox BooksComboBox { get; set; }

        /// <summary>
        /// 控件和视图对象
        /// </summary>
        public struct Views
        {
            public static MainWindow MainWindow { get; set; }
            public static StackPanel EditorShower { get; set; }
            public static HandyControl.Controls.TabControl EditorTabControl { get; set; }
            public static UcShower UcShower { get; set; }
            public static UcSearcher UcSearcher { get; set; }
            public static Border BorderR { get; set; }
            public static MyTree UcNotesTree { get; set; }
            public static MyTree UcMaterials { get; set; }
            public static GroupBox GboxTree { get; set; }
            public static GroupBox GboxWork { get; set; }
            public static Label LbShowNoVerify { get; set; }
            public static Button BtnSettings { get; set; }
            public static Window WSettings { get; set; }
        }


    }
}
