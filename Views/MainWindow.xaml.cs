using RootNS.Helper;
using RootNS.Models;
using RootNS.MyControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RootNS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentBookGuid, Gval.CurrentBook.Guid);
            Gval.CurrentBook.Load();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Gval.Views.EditorTabControl != null)
            {
                while (Gval.Views.EditorTabControl.Items.Count > 0)//集合可能改变，故而不需要i++之类的条件
                {
                    HandyControl.Controls.TabItem tabItem = Gval.Views.EditorTabControl.Items[Gval.Views.EditorTabControl.Items.Count - 1] as HandyControl.Controls.TabItem;
                    tabItem.Focus();
                    Workflow.FindByName(tabItem.CommandBindings, "Close").Execute(tabItem);
                }
            }
            foreach (SqliteHelper cSqlite in SqliteHelper.PoolDict.Values)
            {
                cSqlite.Close();
            }
            if (MyControls.FindReplaceDialog.theDialog != null)
            {
                MyControls.FindReplaceDialog.theDialog.Close();
            }
            Application.Current.Shutdown(0);
        }


    }
}
