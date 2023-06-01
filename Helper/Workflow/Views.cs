using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        /// <summary>
        /// 弹窗位置
        /// </summary>
        /// <param name="thisWin"></param>
        /// <param name="uc"></param>
        public static void ForViewPointX(Window thisWin, double offset = 0)
        {
            thisWin.Left = 300 + offset;
        }
        public static void ForViewPointY(Window thisWin, double offset = 0)
        {
            thisWin.Top = 300 + offset;
        }

        /// <summary>
        /// 更新编辑器中的着色
        /// </summary>
        /// <param name="node"></param>
        public static void UpDataHilgliting(Node node)
        {
            if (Gval.Views.CurrentEditorkernel == null)
            {
                return;
            }
            node.Owner.UpdataSyntax();
        }
    }
}
