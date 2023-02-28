using RootNS.Models;
using RootNS.Views;
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
    /// NameTool.xaml 的交互逻辑
    /// </summary>
    public partial class NameTool : UserControl
    {
        public NameTool()
        {
            InitializeComponent();
        }
        public Name Nameer { get; set; } = new Name();


        private void BtnOpenToolWindow_Click(object sender, RoutedEventArgs e)
        {
            NameToolWindow win = new NameToolWindow();
            win.ShowDialog();
        }

        private void BtnRandomGenerate_Click(object sender, RoutedEventArgs e)
        {
            Nameer.Generate(1);

            string toolTip = string.Empty;
            foreach (char c in BtnResult.Content.ToString())
            {
                string pinyin = Nameer.ReadFromPinyinDict(c.ToString());
                toolTip += pinyin.Trim() + "    ";
            }
            BtnResult.ToolTip = toolTip.Trim();
        }

        private void BtnResult_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(BtnResult.Content.ToString());
            HandyControl.Controls.Growl.SuccessGlobal("已复制名字到剪贴板！");
        }
    }
}
