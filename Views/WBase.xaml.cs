using RootNS.Helper;
using RootNS.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RootNS.Views
{
    /// <summary>
    /// WBase.xaml 的交互逻辑
    /// </summary>
    public partial class WBase : Window
    {
        public WBase()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 允许上传的标记
        /// </summary>
        public bool UpTag { get; set; } = false;
        private void ThisWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as Node).TypeName == Book.TypeNameEnum.云草稿.ToString() &&
                BtnSave.IsEnabled == false && UpTag == true)
            {
                string localFilePath = Gval.Path.DataDirectory + (this.DataContext as Node).Guid + ".txt";
                string remoteFile = Gval.Webdav.Url + "\\" + (this.DataContext as Node).Guid + ".txt";
                FileIO.WriteToTxt(localFilePath, (this.DataContext as Node).Text);
                string eTag = WebdavHelper.UploadWebDavFile(remoteFile, localFilePath, Gval.Webdav.UserName, Gval.Webdav.PassWord, localFilePath);
                if (string.IsNullOrEmpty(eTag))
                {
                    HandyControl.Controls.Growl.Warning("云同步失败，请检查网络或者地址、账号和应用密码");
                }
                else
                {
                    TbSummary.Text = (this.DataContext as Node).Summary = eTag;
                    BtnSave_Click(null, null);
                }
            }
            if ((this.DataContext as Node).TypeName == Book.TypeNameEnum.云大纲.ToString() &&
                BtnSave.IsEnabled == false && UpTag == true)
            {
                System.Collections.ArrayList nodes = new System.Collections.ArrayList();
                nodes.AddRange(Gval.CurrentBook.TabRoot.ChildNodes[2].GetHeirsList());
                nodes.AddRange(Gval.CurrentBook.TabRoot.ChildNodes[0].GetHeirsList());

                string[] rets = Regex.Split((this.DataContext as Node).Text, "(第.+?章.*?\n)");
                char[] myChar = { '：', ' ', '-'};
                string title = string.Empty;
                string content = string.Empty;
                string sql = string.Empty;
                foreach (string str in rets)
                {
                    Match match = Regex.Match(str, "(第.+?章[： ].*?\n)");
                    if (match.Success)
                    {
                        title = match.Value;
                    }
                    else
                    {
                        content = str.Trim();
                    }
                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                    {
                        continue;
                    }
                    string[] words = title.Split(new[] { '\n', '，', ',', ' ', '　', '：', ':' });
                    string firstWord = words[0];
                    Node newNode = new Node
                    {
                        Title = firstWord.Trim(),
                        Text = "　　" + content.Trim(),
                    }; 
                    foreach (Node node in nodes)
                    {                       
                        if (node.Title.Contains(newNode.Title.Trim(myChar)) && node.Summary.Trim().Equals(newNode.Text.Trim()) == false)
                        {
                            sql += string.Format("UPDATE 内容 SET Summary='{0}' WHERE Guid='{1}';", newNode.Text.Replace("'", "''"), node.Guid);
                            if (Gval.Views.UcShower.ThisTextEditor.Text.Equals(node.Summary))
                            {
                                Gval.Views.UcShower.ThisTextEditor.Text = newNode.Text;
                            }
                            foreach (object item in Gval.TextEditorList)
                            {
                                if ((item as ICSharpCode.AvalonEdit.TextEditor).Text.Equals(node.Summary))
                                {
                                    (item as ICSharpCode.AvalonEdit.TextEditor).Text = newNode.Text;
                                }
                            }
                            node.Summary = newNode.Text;
                        }
                    }
                    title = string.Empty;
                    content = string.Empty;
                }
                SqliteHelper.PoolDict[Gval.CurrentBook.Guid.ToString()].ExecuteNonQuery(sql);
                SqliteHelper.PoolDict[Gval.CurrentBook.Guid.ToString()].Close();
                string localFilePath = Gval.Path.DataDirectory + (this.DataContext as Node).Guid + ".txt";
                string remoteFile = Gval.Webdav.Url + "\\" + (this.DataContext as Node).Guid + ".txt";
                FileIO.WriteToTxt(localFilePath, (this.DataContext as Node).Text);
                string eTag = WebdavHelper.UploadWebDavFile(remoteFile, localFilePath, Gval.Webdav.UserName, Gval.Webdav.PassWord, localFilePath);
                if (string.IsNullOrEmpty(eTag))
                {
                    HandyControl.Controls.Growl.Warning("云同步失败，请检查网络或者地址、账号和应用密码");
                }
                else
                {
                    TbSummary.Text = (this.DataContext as Node).Summary = eTag;
                    BtnSave_Click(null, null);
                }
                Console.WriteLine(string.Format("章节大纲保存成功！"));
            }
            Gval.TextEditorList.Remove(LightEditor.ThisTextEditor);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (FunctionsPack.IsInDesignMode(this))
            {
                return;
            }
            LightEditor.DataContext = this.DataContext as Node;
            LightEditor.ThisTextEditor.TextChanged += ThisTextEditor_TextChanged;
            LightEditor.BtnSaveDoc.IsEnabledChanged += BtnSaveDoc_IsEnabledChanged;
            TbTitle.Text = (this.DataContext as Node).Title;
            TbSummary.Text = (this.DataContext as Node).Summary;
            if (string.IsNullOrEmpty((this.DataContext as Node).Text))
            {
                LightEditor.ThisTextEditor.Text = "　　";
            }
            else
            {
                LightEditor.ThisTextEditor.Text = (this.DataContext as Node).Text;
            }
            if ((this.DataContext as Node).TypeName == Book.TypeNameEnum.云草稿.ToString() ||
                (this.DataContext as Node).TypeName == Book.TypeNameEnum.云大纲.ToString())
            {
                this.Width = 700;
                RTitle.Height = new GridLength(0);
                RSummary.Height = new GridLength(0);
            }
            else
            {
                LightEditor.Column1.Width = new GridLength(0);
                LightEditor.Row1.Height = new GridLength(0);
                LightEditor.ThisTextEditor.ShowLineNumbers = false;
                //获取鼠标位置以设置窗口
                Point point = Mouse.GetPosition(Gval.Views.MainWindow);
                this.Left = point.X - this.ActualWidth * 0.618;
                //this.Top = point.Y - 26;
            }
            LightEditor.BtnSaveDoc.IsEnabled = false;
            UpTag = false;
            Gval.TextEditorList.Add(LightEditor.ThisTextEditor);
        }

        private void BtnSaveDoc_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BtnSave.IsEnabled = LightEditor.BtnSaveDoc.IsEnabled;
            UpTag = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //左键按下时，允许拖曳
            DragMove();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LightEditor.BtnSaveDoc.IsEnabled = true;
        }
        private void ThisTextEditor_TextChanged(object sender, EventArgs e)
        {
            LightEditor.BtnSaveDoc.IsEnabled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BtnSave.IsEnabled = false;
            (LightEditor.DataContext as Node).Title = TbTitle.Text;
            (LightEditor.DataContext as Node).Summary = LightEditor.SummaryTextEditor.Text = TbSummary.Text; //利用SummaryTextEditor.Text保存！
            (LightEditor.DataContext as Node).Text = LightEditor.ThisTextEditor.Text;
            LightEditor.BtnSaveText_Click(null, null);
        }



        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSave.IsEnabled == true)
            {
                MessageBoxResult dr = MessageBox.Show("尚未保存\n要在退出前保存更改吗？", "Tip", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
                if (dr == MessageBoxResult.Yes)
                {
                    BtnSave_Click(null, null);
                }
                if (dr == MessageBoxResult.No)
                {

                }
                if (dr == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            this.Close();
        }


        private void BtnSeeLess_Click(object sender, RoutedEventArgs e)
        {
            GSummary.Visibility = Visibility.Collapsed;
            BtnSeeLess.Visibility = Visibility.Collapsed;
            BtnSeeMore.Visibility = Visibility.Visible;
        }

        private void BtnSeeMore_Click(object sender, RoutedEventArgs e)
        {
            GSummary.Visibility = Visibility.Visible;
            BtnSeeMore.Visibility = Visibility.Collapsed;
            BtnSeeLess.Visibility = Visibility.Visible;
        }


    }
}
