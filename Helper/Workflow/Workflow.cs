using RootNS.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace RootNS.Helper
{
    /// <summary>
    /// 工作流（内部业务）
    /// </summary>
    internal partial class Workflow
    {
        /// <summary>
        /// 根据名称查找命令
        /// </summary>
        /// <param name="commandBindings"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public static ICommand FindByName(CommandBindingCollection commandBindings, string commandName)
        {
            foreach (CommandBinding cb in commandBindings)
            {
                RoutedCommand rc = cb.Command as RoutedCommand;
                if (commandName == rc.Name)
                {
                    return cb.Command;
                }
            }
            return null;
        }




        //-----------------------------------------------以下为私有方法-----------------------------------------------





    }
}
