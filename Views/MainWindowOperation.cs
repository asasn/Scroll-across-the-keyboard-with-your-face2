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
    public partial class MainWindow : Window
    {
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentBookGuid, Gval.CurrentBook.Guid);
            Gval.CurrentBook.Load();
            Gval.FlagLoadingCompleted = true;
            RbEvents.IsChecked = false;
            RbEvents.IsChecked = true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        /// <summary>
        /// 点击：恢复默认的主面板布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReLayout_Click(object sender, RoutedEventArgs e)
        {
            ColTree.Width = new GridLength(264, GridUnitType.Pixel);
            ColEditor.Width = new GridLength(700, GridUnitType.Pixel);
        }

        private void BtnThemeColor_Click(object sender, RoutedEventArgs e)
        {
            Random ran = new Random();
            byte a = (byte)(ran.Next(180, 256));
            byte r = (byte)(ran.Next(0, 256));
            byte g = (byte)(ran.Next(0, 256));
            byte b = (byte)(ran.Next(0, 256));
            Brush brush1 = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            Gval.CurrentThemeColor = (SolidColorBrush)brush1;
            Brush brush2 = new SolidColorBrush(Color.FromArgb(a,
                (byte)(255 - r), (byte)(255 - g), (byte)(255 - b)));
            Resources["PrimaryBrush"] = (SolidColorBrush)brush2;
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentThemeColor, brush1.ToString());
        }

        private void BtnThemeColor_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Brush brush2 = new SolidColorBrush(Color.FromArgb(Gval.DefaultThemeColor.Color.A,
                (byte)(255 - Gval.DefaultThemeColor.Color.R),
                (byte)(255 - Gval.DefaultThemeColor.Color.G),
                (byte)(255 - Gval.DefaultThemeColor.Color.B)));
            Resources["PrimaryBrush"] = (SolidColorBrush)brush2;
            Gval.CurrentThemeColor = Gval.DefaultThemeColor;
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentThemeColor, Gval.DefaultThemeColor.ToString());
        }

        private void BtnAddABook_Click(object sender, RoutedEventArgs e)
        {
            Workflow.CreadANewBook();
        }

        private void BtnDelCurrentBook_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEditCurrentBookInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RbEvents_Checked(object sender, RoutedEventArgs e)
        {
            NotesTree.DataContext = Gval.CurrentBook.TabRoot.ChildNodes[3];
        }

        private void RbSnippetsInBook_Checked(object sender, RoutedEventArgs e)
        {
            NotesTree.DataContext = Gval.CurrentBook.TabRoot.ChildNodes[4];
        }

        private void RbCardsInBook_Checked(object sender, RoutedEventArgs e)
        {
            NotesTree.DataContext = Gval.CurrentBook.TabRoot.ChildNodes[5];
        }

        private void RbPlotDesign_Checked(object sender, RoutedEventArgs e)
        {
            MaterialsTree.DataContext = Gval.MaterialBook.TabRoot.ChildNodes[6];
        }

        private void RbSnippets_Checked(object sender, RoutedEventArgs e)
        {
            MaterialsTree.DataContext = Gval.MaterialBook.TabRoot.ChildNodes[7];
        }

        private void RbCards_Checked(object sender, RoutedEventArgs e)
        {
            MaterialsTree.DataContext = Gval.MaterialBook.TabRoot.ChildNodes[8];
        }


        private void RbMaterial_Checked(object sender, RoutedEventArgs e)
        {
            MaterialsTree.DataContext = Gval.MaterialBook.TabRoot.ChildNodes[9];
        }
        private void RbTopic_Checked(object sender, RoutedEventArgs e)
        {
            MaterialsTree.DataContext = Gval.MaterialBook.TabRoot.ChildNodes[10];
        }

        private void RbInspiration_Checked(object sender, RoutedEventArgs e)
        {
            MaterialsTree.DataContext = Gval.MaterialBook.TabRoot.ChildNodes[11];
        }


        private void BtnBankManage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
