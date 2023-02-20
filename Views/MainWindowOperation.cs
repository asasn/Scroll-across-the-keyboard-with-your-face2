﻿using RootNS.Helper;
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
        /// <summary>
        /// 点击：恢复默认的主面板布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReLayout_Click(object sender, RoutedEventArgs e)
        {
            ColTree.Width = new GridLength(240, GridUnitType.Pixel);
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

        private void RbHistory_Checked(object sender, RoutedEventArgs e)
        {
            Notes.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[3];
            Notes.BtnFolder.IsEnabled = true;
        }

        private void RbStory_Checked(object sender, RoutedEventArgs e)
        {
            Notes.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[4];
            Notes.BtnFolder.IsEnabled = true;
        }

        private void RbSnippetsInBook_Checked(object sender, RoutedEventArgs e)
        {
            Notes.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[5];
            Notes.BtnFolder.IsEnabled = true;
        }

        private void RbPlotDesignInBook_Checked(object sender, RoutedEventArgs e)
        {
            Notes.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[6];
            Notes.BtnFolder.IsEnabled = true;
        }

        private void RbCardsInBook_Checked(object sender, RoutedEventArgs e)
        {
            Notes.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[7];
            Notes.BtnFolder.IsEnabled = false;
        }



        private void RbMaterial_Checked(object sender, RoutedEventArgs e)
        {
            Materials.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[8];
        }
        private void RbTopic_Checked(object sender, RoutedEventArgs e)
        {
            Materials.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[9];
        }
        private void RbSnippets_Checked(object sender, RoutedEventArgs e)
        {
            Materials.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[10];
        }

        private void RbPlotDesign_Checked(object sender, RoutedEventArgs e)
        {
            Materials.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[11];
        }
        private void RbCards_Checked(object sender, RoutedEventArgs e)
        {
            Materials.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[12];
        }

        private void RbInspiration_Checked(object sender, RoutedEventArgs e)
        {
            Materials.DataContext = Gval.CurrentBook.TreeRoot.ChildNodes[13];
        }


        private void BtnBankManage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}