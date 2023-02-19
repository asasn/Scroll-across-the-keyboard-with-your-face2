using RootNS.Helper;
using RootNS.Models;
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

namespace RootNS.MyControls
{
    /// <summary>
    /// TomatoClock.xaml 的交互逻辑
    /// </summary>
    public partial class TomatoClock : UserControl
    {
        public TomatoClock()
        {
            InitializeComponent();
        }

        public Models.TomatoClock ThisClock { get; set; } = new Models.TomatoClock();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ThisClock.MeDida = MeDida;
            ThisClock.MeRing = MeRing;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            ThisClock.Start();
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            if (ThisClock.IsSetting == Visibility.Visible)
            {
                ThisClock.IsSetting = Visibility.Collapsed;
            }
            else
            {
                ThisClock.IsSetting = Visibility.Visible;
            }
        }

        private void CbTime_Loaded(object sender, RoutedEventArgs e)
        {
            ThisClock.CbTime_Loaded();
        }
    }
}
