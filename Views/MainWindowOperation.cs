using RootNS.Helper;
using RootNS.Models;
using RootNS.MyControls;
using RootNS.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if (Gval.CurrentBook == null)
            {
                return;
            }
            Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.CurrentBookGuid, Gval.CurrentBook.Guid);
            if (Gval.FlagLoadingCompleted == true)
            {
                Gval.FlagLoadingCompleted = false;
                Gval.CurrentBook.Load();
                Gval.FlagLoadingCompleted = true;
                Gval.Views.UcSearcher.TbKeyWords.Clear();
                Gval.Views.UcSearcher.ThisSearcher.Start();
            }
            RbEventsInBook.IsChecked = false;
            RbEventsInBook.IsChecked = true;
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
            if (SqliteHelper.PoolDict.ContainsKey(Gval.CurrentBook.Guid.ToString()) == false)
            {
                return;
            }
            WDelBook wDelBook = new WDelBook();
            wDelBook.Show();
        }

        private void BtnEditCurrentBookInfo_Click(object sender, RoutedEventArgs e)
        {
            if (SqliteHelper.PoolDict.ContainsKey(Gval.CurrentBook.Guid.ToString()) == false)
            {
                return;
            }
            WBookInfo wBookInfo = new WBookInfo();
            wBookInfo.Show();
        }

        private void RbEventsInBook_Checked(object sender, RoutedEventArgs e)
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

        private void BtnOutlines_Click(object sender, RoutedEventArgs e)
        {
            if (SqliteHelper.PoolDict.ContainsKey(Gval.CurrentBook.Guid.ToString()) == false)
            {
                return;
            }
            Node nodeShell = null;
            if (Gval.CurrentBook.TabRoot.ChildNodes[13].ChildNodes.Count == 0)
            {
                nodeShell = new Node();
                Gval.CurrentBook.TabRoot.ChildNodes[13].ChildNodes.Add(nodeShell);
                nodeShell.Insert();
            }
            else
            {
                nodeShell = Gval.CurrentBook.TabRoot.ChildNodes[13].ChildNodes[0];
            }
            WBase wBase = new WBase() { DataContext = nodeShell };
            string localFilePath = Gval.Path.DataDirectory + nodeShell.Guid + ".txt";
            string remoteFile = Gval.Webdav.Url + "\\" + nodeShell.Guid + ".txt";
            string eTag = WebdavHelper.GetEtag(remoteFile, Gval.Webdav.UserName, Gval.Webdav.PassWord);
            if (nodeShell.Summary != eTag)
            {
                byte[] buffer = WebdavHelper.DownloadWebDavFile(remoteFile, Gval.Webdav.UserName, Gval.Webdav.PassWord, localFilePath);
                if (buffer == null)
                {
                    HandyControl.Controls.Growl.Warning("云同步失败，请在设置——Webdav选项中设置地址、账号和应用密码，并确认网络畅通。");
                }
                else
                {
                    File.WriteAllBytes(localFilePath, buffer);
                }
            }
            if (FileIO.IsFileExists(localFilePath))
            {
                nodeShell.Text = FileIO.ReadFromTxt(localFilePath);
            }
            else
            {
                string text = "　　";
                System.Collections.ArrayList nodes = new System.Collections.ArrayList();
                nodes.AddRange(Gval.CurrentBook.TabRoot.ChildNodes[2].GetHeirsList());
                nodes.AddRange(Gval.CurrentBook.TabRoot.ChildNodes[0].GetHeirsList());
                foreach (Node node in nodes)
                {
                    string title = Workflow.GetTitleFromTitle(node.Title);
                    if (string.IsNullOrEmpty(title) == false)
                    {
                        text += title + "：\n　　\n" + node.Summary + "\n　　\n　　";
                    }
                }
                nodeShell.Text = text;
            }
            if (string.IsNullOrEmpty(nodeShell.Text.Trim()))
            {
                nodeShell.Text = "　　第1章：测试标题1\n　　\n　　测试的章纲文字测试的章纲文字测试的章纲文字。\n　　\n　　第2章：测试标题2\n　　\n　　测试的章纲文字测试的章纲文字测试的章纲文字。\n　　\n　　第3章：测试标题3\n　　\n　　测试的章纲文字测试的章纲文字测试的章纲文字。";
            }
            wBase.Show();
        }

        private void BtnReadmodel_Click(object sender, RoutedEventArgs e)
        {
            if (Gval.CurrentBook.TabRoot.ChildNodes[0].ChildsCount == 0)
            {
                return;
            }
            string content = string.Empty;
            foreach (Node node in Gval.CurrentBook.TabRoot.ChildNodes[0].ChildNodes)
            {
                content += node.Title.Trim() + "\n";
                content += node.Text + "\n\n";
            }
            Views.WShowPackage wShow = new Views.WShowPackage();
            wShow.ThisTextEditor.Text = content;
            wShow.ThisTextEditor.ShowLineNumbers = false;
            wShow.ThisTextEditor.FontSize = 28;
            wShow.ThisTextEditor.FontFamily = new FontFamily("霞鹜文楷");
            wShow.Show();
        }

        private void BtnPackage_Click(object sender, RoutedEventArgs e)
        {
            if (SqliteHelper.PoolDict.ContainsKey(Gval.CurrentBook.Guid.ToString()) == false)
            {
                return;
            }
            string content = string.Empty;
            foreach (Node card in Gval.CurrentBook.TabRoot.ChildNodes[5].GetHeirsList())
            {
                if (card.Attachment == null || card.Card == null || card.IsDir == true)
                {
                    continue;
                }
                if (card.Card.Tag.Equals("错误"))
                {
                    continue;
                }
                content += card.Title.Trim() + "\n";
                foreach (Card.Line.Tip tip in card.Card.Lines[0].Tips)
                {
                    content += tip.Content.Trim() + "\n";
                }
            }
            Views.WShowPackage wShow = new Views.WShowPackage();
            wShow.ThisTextEditor.Text = content;
            wShow.Show();
        }

        private void BtnChatAI_Click(object sender, RoutedEventArgs e)
        {
            Views.WChatAI chatAI = new Views.WChatAI();
            chatAI.Show();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Views.WSettings wSettings = new Views.WSettings();
            wSettings.Show();
        }


        private void BtnCloudDocument_Click(object sender, RoutedEventArgs e)
        {
            Node node = null;
            if (Gval.MaterialBook.TabRoot.ChildNodes[12].ChildNodes.Count == 0)
            {
                node = new Node();
                Gval.MaterialBook.TabRoot.ChildNodes[12].ChildNodes.Add(node);
                node.Insert();
            }
            else
            {
                node = Gval.MaterialBook.TabRoot.ChildNodes[12].ChildNodes[0];
            }
            WBase wBase = new WBase() { DataContext = node };
            string localFilePath = Gval.Path.DataDirectory + node.Guid + ".txt";
            string remoteFile = Gval.Webdav.Url + "\\" + node.Guid + ".txt";
            string eTag = WebdavHelper.GetEtag(remoteFile, Gval.Webdav.UserName, Gval.Webdav.PassWord);
            if (node.Summary != eTag)
            {
                byte[] buffer = WebdavHelper.DownloadWebDavFile(remoteFile, Gval.Webdav.UserName, Gval.Webdav.PassWord, localFilePath);
                if (buffer == null)
                {
                    HandyControl.Controls.Growl.Warning("云同步失败，请在设置——Webdav选项中设置地址、账号和应用密码，并确认网络畅通。");
                }
                else
                {
                    File.WriteAllBytes(localFilePath, buffer);
                }
            }
            if (FileIO.IsFileExists(localFilePath))
            {
                node.Text = FileIO.ReadFromTxt(localFilePath);
            }
            wBase.Show();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            WRegister wHelp = new WRegister();
            wHelp.Show();
        }
        private void BtnHmoepage_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Properties.Resources.Homepage);
        }
    }
}
