using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RootNS.Helper
{
    internal partial class Workflow
    {
        /// <summary>
        /// 弹窗位置
        /// </summary>
        /// <param name="thisWin"></param>
        /// <param name="uc"></param>
        public static void ForViewPointX(Window thisWin, UIElement uc, double offset = 0)
        {
            thisWin.Left = uc.TranslatePoint(new Point(), Gval.Views.MainWindow).X + offset;
        }
        public static void ForViewPointY(Window thisWin, UIElement uc, double offset = 0)
        {
            thisWin.Top = uc.TranslatePoint(new Point(), Gval.Views.MainWindow).Y + offset;
        }
    }
}
